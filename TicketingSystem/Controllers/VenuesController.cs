using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Cosmos;
using TicketingSystem.Models;

namespace TicketingSystem.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class VenuesController : ControllerBase
    {
        private readonly Database database;
        public VenuesController(Database db)
        {
            database = db;
        }
        //return all venues
        [HttpGet]
        public async Task<IActionResult> GetVenues()
        {
            var container = database.GetContainer("Venues");
            var venues = await container.QueryAsync<Venue>(x => true);
            return new OkObjectResult(venues);
        }

        //return available venues date is yyyy-MM-dd format
        [HttpGet]
        [Route("Available/{date}")]
        public async Task<IActionResult> GetAvailableVenues([FromRoute] string date)
        {
            var container = database.GetContainer("Venues");
            var eventsContainer = database.GetContainer("Events");

            var venues = await container.QueryAsync<Venue>(x => true);
            List<Venue> availableVenues = new List<Venue>();
            foreach (var venue in venues)
            {
                var events = await eventsContainer.QueryAsync<Event>(x => x.VenueId == venue.Id && x.StartDate.StartsWith(date));
                if (events.Count == 0)
                {
                    availableVenues.Add(venue);
                }
            }
            return new OkObjectResult(venues);
        }

        [HttpPut]
        public async Task<IActionResult> CreateAVenue([FromBody] VenueModel venueModel)
        {
            var venue = new Venue
            {
                Id = Guid.NewGuid().ToString(),
                Name = venueModel.Name,
                Description = venueModel.Description,
                MaxCapacity = venueModel.MaxCapacity
            };
            var container = database.GetContainer("Venues");
            await container.UpsertItemAsync(venue);
            return new OkObjectResult(venue);
        }

        [HttpPost]
        public async Task<IActionResult> UpdateAVenue([FromBody] Venue venue)
        {
            var venuesContainer = database.GetContainer("Venues");
            var eventsContainer = database.GetContainer("Events");
            var ticketsContainer = database.GetContainer("Tickets");
            var venues = await venuesContainer.QueryAsync<Venue>(x => x.Id == venue.Id);
            if (venues.Count == 0)
            {
                return new NotFoundObjectResult("No venue found");
            }
            var events = await eventsContainer.QueryAsync<Event>(x => x.VenueId == venue.Id);
            foreach (var even in events)
            {
                var tickets = await ticketsContainer.QueryAsync<Ticket>(x => x.EventId == even.Id && x.IsSold);
                if (tickets.Count > venue.MaxCapacity)
                {
                    return new BadRequestObjectResult("Can't reduce capacity to less than tickets sold for any event");
                }
            }


            await venuesContainer.DeleteItemAsync<Venue>(venues[0].Id, new PartitionKey(venues[0].Name));
            await venuesContainer.UpsertItemAsync(venue);
            return new OkObjectResult(venue);
        }
    }
}
