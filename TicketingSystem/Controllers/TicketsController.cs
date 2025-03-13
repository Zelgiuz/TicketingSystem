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
        public async Task<IActionResult> Index()
        {
            return new OkResult();
        }

        [HttpGet]
        [Route("/event/{eventId}/tickets")]
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
            var availableTickets = tickets.Where(x => (!x.IsReserved || x.ReservedUntilDateTime.FromISO8601() < DateTime.Now) && !x.IsSold).ToList();
            return new OkObjectResult(availableTickets);
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

            if (ticketsToReserve.Any(x => x.IsReserved && (x.ReservedUntilDateTime.FromISO8601() > DateTime.Now.ToIso8601().FromISO8601())))
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
                ticket.ReservedUntilDateTime = DateTime.Now.AddMinutes(15).ToIso8601();
                await ticketsContainer.UpsertItemAsync(ticket);
            }
            return new OkObjectResult(ticketsToReserve);
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
                ticket.ReservedUntilDateTime = DateTime.MinValue.ToIso8601();
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
            if (ticketsToBuy.Any(x => x.IsSold || !x.IsReserved || x.ReservedUser != userId || x.ReservedUntilDateTime.FromISO8601() >= DateTime.Now))
            {
                return new BadRequestObjectResult("Some of the tickets aren't reserved(the reservation may have expired) or are sold");
            }

            //THIS IS WHERE SOME LOGIC INVOLVING PAYMENT WOULD GO
            //Something Like this
            //var payment = await PaymentService.PayForTickets(ticketsToReserve, userId);


            foreach (var ticket in ticketsToBuy)
            {
                ticket.IsSold = true;
                ticket.IsReserved = false;
                ticket.ReservedUser = -1;
                ticket.ReservedUntilDateTime = DateTime.MinValue.ToIso8601();
                await ticketsContainer.UpsertItemAsync(ticket);
            }
            return new OkObjectResult(ticketsToBuy);
        }
    }
}
