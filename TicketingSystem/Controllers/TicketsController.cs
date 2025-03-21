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
        [Route("/event/{eventId}/tickets/reserve")]
        public async Task<IActionResult> ReserveTickets([FromRoute] string eventId, [FromRoute] int userId, [FromBody] TicketsToChangeModel ticketsToChange)
        {
            var eventsContainer = database.GetContainer("Events");
            var ticketsContainer = database.GetContainer("Tickets");
            var anEvent = (await eventsContainer.QueryAsync<Event>(x => x.Id == eventId)).First();
            if (anEvent == null)
            {
                return new NotFoundObjectResult("No event found");
            }

            var tickets = (await ticketsContainer.QueryAsync<Ticket>(x => x.EventId == eventId && ticketsToChange.Tickets.Any(y => y.SeatNumber == x.SeatNumber && y.SectionId == x.SectionId))).ToList();

            if (tickets.Count == 0)
            {
                return new NotFoundObjectResult("No tickets found");
            }

            if (tickets.Any(x => x.IsReserved && (x.ReservedUntilDateTime.FromISO8601() > DateTime.UtcNow)))
            {
                return new BadRequestObjectResult("Tickets are already reserved");
            }
            if (tickets.Any(x => x.IsSold))
            {
                return new BadRequestObjectResult("Tickets are already sold");
            }

            foreach (var ticket in tickets)
            {
                ticket.IsReserved = true;
                ticket.ReservedUser = userId;
                ticket.ReservedUntilDateTime = DateTime.UtcNow.AddMinutes(15).ToISO8601();
                await ticketsContainer.UpsertItemAsync(ticket);
            }
            decimal price = 0;
            foreach (var ticket in tickets)
            {
                var section = anEvent.Sections.FirstOrDefault(x => x.Id == ticket.SectionId);
                if (section != null)
                {
                    price += section.Price;
                }
            }
            return new OkObjectResult(new ReservedTicketsModel(tickets, price));
        }

        [HttpPost]
        [Route("/event/{eventId}/tickets/cancelreservation")]
        public async Task<IActionResult> CancelReservationTickets([FromRoute] string eventId, [FromBody] TicketsToChangeModel ticketsToChange)
        {
            var eventsContainer = database.GetContainer("Events");
            var ticketsContainer = database.GetContainer("Tickets");
            var events = await eventsContainer.QueryAsync<Event>(x => x.Id == eventId);
            if (events.Count == 0)
            {
                return new NotFoundObjectResult("No event found");
            }
            var tickets = (await ticketsContainer.QueryAsync<Ticket>(x => x.EventId == eventId && ticketsToChange.Tickets.Any(y => y.SeatNumber == x.SeatNumber && y.SectionId == x.SectionId && x.IsReserved && x.ReservedUser == ticketsToChange.UserId))).ToList();
            //Cancel only the tickets that are reserved by the user
            ;
            if (tickets.Count == 0)
            {
                return new NotFoundObjectResult("No tickets found");
            }
            foreach (var ticket in tickets)
            {
                ticket.IsReserved = false;
                ticket.ReservedUser = -1;
                ticket.ReservedUntilDateTime = DateTime.MinValue.ToISO8601();
                await ticketsContainer.UpsertItemAsync(ticket);
            }
            return new OkObjectResult(tickets);
        }

        [HttpPost]
        [Route("/event/{eventId}/tickets/buy")]
        public async Task<IActionResult> BuyTickets([FromRoute] string eventId, [FromBody] TicketsToChangeModel ticketsToChange)
        {
            var eventsContainer = database.GetContainer("Events");
            var ticketsContainer = database.GetContainer("Tickets");
            var events = await eventsContainer.QueryAsync<Event>(x => x.Id == eventId);
            if (events.Count == 0)
            {
                return new NotFoundObjectResult("No event found");
            }
            var eventToCheck = events.First();
            var tickets = await ticketsContainer.QueryAsync<Ticket>(x => x.EventId == eventToCheck.Id && ticketsToChange.Tickets.Any(y => y.SeatNumber == x.SeatNumber && y.SectionId == x.SectionId));
            if (tickets.Count == 0)
            {
                return new NotFoundObjectResult("No tickets found");
            }
            if (tickets.Any(x => x.IsSold || !x.IsReserved || x.ReservedUser != ticketsToChange.UserId || !(x.ReservedUntilDateTime.FromISO8601() > DateTime.UtcNow)))
            {
                return new BadRequestObjectResult("Some of the tickets aren't reserved(the reservation may have expired) or are sold");
            }

            decimal price = 0;
            foreach (var ticket in tickets)
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


            foreach (var ticket in tickets)
            {
                ticket.IsSold = true;
                ticket.IsReserved = false;
                ticket.ReservedUser = -1;
                ticket.ReservedUntilDateTime = DateTime.MinValue.ToISO8601();
                await ticketsContainer.UpsertItemAsync(ticket);
            }
            return new OkObjectResult(tickets);
        }
    }
}
