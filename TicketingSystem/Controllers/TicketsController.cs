using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Cosmos;
using TicketingSystem.Extensions;
using TicketingSystem.Models;

namespace TicketingSystem.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class TicketsController : ControllerBase
    {
        private readonly Database database;
        public TicketsController(Database db)
        {
            database = db;
        }
        [HttpGet]
        public async Task<IActionResult> GetAllTickets()
        {
            var ticketsContainer = database.GetContainer("Tickets");
            var tickets = await ticketsContainer.QueryAsync<Ticket>(x => true);
            return new OkObjectResult(tickets);
        }

        [HttpGet]
        [Route("/event/{eventId}/tickets/available")]
        public async Task<IActionResult> GetAvailableEventTickets([FromRoute] string eventId)
        {
            var eventsContainer = database.GetContainer("Events");
            var ticketsContainer = database.GetContainer("Tickets");
            var events = await eventsContainer.QueryAsync<Event>(x => x.Id == eventId);
            if (events.Count == 0)
            {
                return new NotFoundObjectResult("No event found");
            }
            var eventToCheck = events.First();
            var tickets = await ticketsContainer.QueryAsync<Ticket>(x => x.EventId == eventToCheck.Id);
            var availableTickets = tickets.Where(x => !x.IsSold && !(x.ReservedUntilDateTime.FromISO8601() > DateTime.UtcNow.ToISO8601().FromISO8601()));
            return new OkObjectResult(availableTickets);
        }

        [HttpGet]
        [Route("/event/{eventId}/tickets/")]
        public async Task<IActionResult> GetEventTickets([FromRoute] string eventId)
        {
            var eventsContainer = database.GetContainer("Events");
            var ticketsContainer = database.GetContainer("Tickets");
            var events = await eventsContainer.QueryAsync<Event>(x => x.Id == eventId);
            if (events.Count == 0)
            {
                return new NotFoundObjectResult("No event found");
            }
            var eventToCheck = events.First();
            var tickets = await ticketsContainer.QueryAsync<Ticket>(x => x.EventId == eventToCheck.Id);
            return new OkObjectResult(tickets);
        }

        [HttpPost]
        [Route("/event/{eventId}/tickets/user/{userId}/reserve")]
        public async Task<IActionResult> ReserveTickets([FromRoute] string eventId, [FromRoute] int userId, [FromBody] TicketsToChangeModel ticketsToChange)
        {
            var eventsContainer = database.GetContainer("Events");
            var ticketsContainer = database.GetContainer("Tickets");
            var events = await eventsContainer.QueryAsync<Event>(x => x.Id == eventId);
            if (events.Count == 0)
            {
                return new NotFoundObjectResult("No event found");
            }
            var eventToCheck = events.First();
            var tickets = await ticketsContainer.QueryAsync<Ticket>(x => x.EventId == eventToCheck.Id);
            var ticketsToReserve = tickets.Where(x => ticketsToChange.Tickets.Any(y => y.SeatNumber == x.SeatNumber && y.SectionId == x.SectionId)).ToList();
            if (ticketsToReserve.Count == 0)
            {
                return new NotFoundObjectResult("No tickets found");
            }

            if (ticketsToReserve.Any(x => x.IsReserved && (x.ReservedUntilDateTime.FromISO8601() > DateTime.UtcNow.ToISO8601().FromISO8601())))
            {
                return new BadRequestObjectResult("Tickets are already reserved");
            }
            if (ticketsToReserve.Any(x => x.IsSold))
            {
                return new BadRequestObjectResult("Tickets are already sold");
            }

            foreach (var ticket in ticketsToReserve)
            {
                ticket.IsReserved = true;
                ticket.ReservedUser = userId;
                ticket.ReservedUntilDateTime = DateTime.UtcNow.AddMinutes(15).ToISO8601();
                await ticketsContainer.UpsertItemAsync(ticket);
            }
            decimal price = 0;
            foreach (var ticket in ticketsToReserve)
            {
                var section = eventToCheck.Sections.FirstOrDefault(x => x.Id == ticket.SectionId);
                if (section != null)
                {
                    price += section.Price;
                }
            }
            return new OkObjectResult(new ReservedTicketsModel(ticketsToReserve, price));
        }

        [HttpPost]
        [Route("/event/{eventId}/tickets/user/{userId}/cancelreservation")]
        public async Task<IActionResult> CancelReservationTickets([FromRoute] string eventId, [FromRoute] int userId, [FromBody] TicketsToChangeModel ticketsToChange)
        {
            var eventsContainer = database.GetContainer("Events");
            var ticketsContainer = database.GetContainer("Tickets");
            var events = await eventsContainer.QueryAsync<Event>(x => x.Id == eventId);
            if (events.Count == 0)
            {
                return new NotFoundObjectResult("No event found");
            }
            var eventToCheck = events.First();
            var tickets = await ticketsContainer.QueryAsync<Ticket>(x => x.EventId == eventToCheck.Id);
            //Cancel only the tickets that are reserved by the user
            var ticketsToReserve = tickets.Where(x => ticketsToChange.Tickets.Any(y => y.SeatNumber == x.SeatNumber && y.SectionId == x.SectionId && x.IsReserved && x.ReservedUser == userId)).ToList();
            if (ticketsToReserve.Count == 0)
            {
                return new NotFoundObjectResult("No tickets found");
            }
            foreach (var ticket in ticketsToReserve)
            {
                ticket.IsReserved = false;
                ticket.ReservedUser = -1;
                ticket.ReservedUntilDateTime = DateTime.MinValue.ToISO8601();
                await ticketsContainer.UpsertItemAsync(ticket);
            }
            return new OkObjectResult(ticketsToReserve);
        }

        [HttpPost]
        [Route("/event/{eventId}/tickets/user/{userId}/buy")]
        public async Task<IActionResult> BuyTickets([FromRoute] string eventId, [FromRoute] int userId, [FromBody] TicketsToChangeModel ticketsToChange)
        {
            var eventsContainer = database.GetContainer("Events");
            var ticketsContainer = database.GetContainer("Tickets");
            var events = await eventsContainer.QueryAsync<Event>(x => x.Id == eventId);
            if (events.Count == 0)
            {
                return new NotFoundObjectResult("No event found");
            }
            var eventToCheck = events.First();
            var tickets = await ticketsContainer.QueryAsync<Ticket>(x => x.EventId == eventToCheck.Id);
            var ticketsToBuy = tickets.Where(x => ticketsToChange.Tickets.Any(y => y.SeatNumber == x.SeatNumber && y.SectionId == x.SectionId)).ToList();
            if (ticketsToBuy.Count == 0)
            {
                return new NotFoundObjectResult("No tickets found");
            }
            if (ticketsToBuy.Any(x => x.IsSold || !x.IsReserved || x.ReservedUser != userId || !(x.ReservedUntilDateTime.FromISO8601() > DateTime.UtcNow.ToISO8601().FromISO8601())))
            {
                return new BadRequestObjectResult("Some of the tickets aren't reserved(the reservation may have expired) or are sold");
            }

            decimal price = 0;
            foreach (var ticket in ticketsToBuy)
            {
                var section = eventToCheck.Sections.FirstOrDefault(x => x.Id == ticket.SectionId);
                if (section != null)
                {
                    price += section.Price;
                }
            }

            //THIS IS WHERE SOME LOGIC INVOLVING PAYMENT WOULD GO
            //Something Like this
            //var payment = await PaymentService.PayForTickets(ticketsToReserve, userId,price);


            foreach (var ticket in ticketsToBuy)
            {
                ticket.IsSold = true;
                ticket.IsReserved = false;
                ticket.ReservedUser = -1;
                ticket.ReservedUntilDateTime = DateTime.MinValue.ToISO8601();
                await ticketsContainer.UpsertItemAsync(ticket);
            }
            return new OkObjectResult(ticketsToBuy);
        }
    }
}
