using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Cosmos;
using TicketingSystem.Models;

namespace TicketingSystem.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class EventsController : ControllerBase
    {
        private readonly Database database;
        public EventsController(Database db)
        {
            database = db;
        }

        [HttpPut]
        public async Task<IActionResult> MakeAnEvent([FromBody] EventModel eventMaker)
        {
            Event result = new Event();
            try
            {
                if (eventMaker == null || eventMaker.Sections.Count < 1)
                    return new BadRequestObjectResult("Must have a valid event with at least one section");
                if (eventMaker.StartDate == null || eventMaker.Name == null || eventMaker.VenueId == null)
                    return new BadRequestObjectResult("Must have a valid event with a name, start date, and venue id");




                var container = database.GetContainer("Venues");
                var eventsContainer = database.GetContainer("Events");

                var events = await eventsContainer.QueryAsync<Event>(x => x.VenueId == eventMaker.VenueId && x.StartDate.StartsWith(eventMaker.StartDate));
                if (events.Count >= 0)
                {
                    return new BadRequestObjectResult("That venue is already booked on that day");
                }

                var ticketContainer = database.GetContainer("Tickets");
                result = eventMaker.CreateEvent();

                await eventsContainer.UpsertItemAsync(result);
                List<Task> tasks = new List<Task>();
                foreach (var section in result.Sections)
                {
                    if (section != null)
                    {
                        for (int i = 0; i < section.SectionCapacity; i++)
                        {
                            Ticket ticket = new Ticket(section, i);
                            tasks.Add(ticketContainer.UpsertItemAsync(ticket));
                        }
                    }
                }
                await Task.WhenAll(tasks);



            }
            catch (Exception ex)
            {
                return new BadRequestObjectResult(ex);
            }
            return new OkObjectResult(result);



        }


        //
        [HttpPost]
        public async Task<IActionResult> UpdateEvent([FromBody] Event updatedEvent)
        {
            try
            {
                if (updatedEvent == null || updatedEvent.Sections.Count < 1)
                    return new BadRequestObjectResult("Must have a valid event with at least one section");
                if (updatedEvent.StartDate == null || updatedEvent.Name == null || updatedEvent.VenueId == null)
                    return new BadRequestObjectResult("Must have a valid event with a name, start date, and venue id");
                var container = database.GetContainer("Venues");
                var eventsContainer = database.GetContainer("Events");
                var events = await eventsContainer.QueryAsync<Event>(x => x.Id == updatedEvent.Id);
                if (events.Count == 0)
                {
                    return new BadRequestObjectResult("Event not found");
                }

                events = await eventsContainer.QueryAsync<Event>(x => x.VenueId == updatedEvent.VenueId && x.StartDate.StartsWith(updatedEvent.StartDate) && x.Id != updatedEvent.Id);
                if (events.Count >= 0)
                {
                    return new BadRequestObjectResult("Can not make the requested change that venue is booked by another event on that day");
                }


                var ticketContainer = database.GetContainer("Tickets");

                await eventsContainer.UpsertItemAsync(updatedEvent);
                List<Task> tasks = new List<Task>();
                foreach (var section in updatedEvent.Sections)
                {
                    if (section != null)
                    {
                        //Add tickets If Needed
                        for (int i = 0; i < section.SectionCapacity; i++)
                        {
                            Ticket ticket = new Ticket(section, i);
                            var tickets = await ticketContainer.QueryAsync<Ticket>(x => x.EventId == updatedEvent.Id && x.SectionId == section.Id && x.SeatNumber == i);
                            if (tickets.Count == 0)
                                tasks.Add(ticketContainer.UpsertItemAsync(ticket));
                        }
                        //Delete Tickets if needed
                        var ticketsToDelete = await ticketContainer.QueryAsync<Ticket>(x => x.EventId == updatedEvent.Id && x.SectionId == section.Id && x.SeatNumber >= section.SectionCapacity);
                        foreach (var ticket in ticketsToDelete)
                        {
                            tasks.Add(ticketContainer.DeleteItemAsync<Ticket>(ticket.Id, new PartitionKey(ticket.SectionId)));
                        }
                    }
                    await Task.WhenAll(tasks);
                }
            }
            catch (Exception ex)
            {
                return new BadRequestObjectResult(ex);
            }
            return new OkObjectResult(updatedEvent);
        }

        [HttpGet]
        public async Task<IActionResult> GetEvents()
        {
            var eventContainer = database.GetContainer("Events");
            List<Event> results = await eventContainer.QueryAsync<Event>(x => true);
            return new OkObjectResult(results);
        }

        [HttpGet]
        [Route("{name}")]
        public async Task<IActionResult> GetEvent([FromRoute] string name)
        {
            var eventContainer = database.GetContainer("Events");
            List<Event> results = await eventContainer.QueryAsync<Event>(x => x.Name == name);
            return new OkObjectResult(results);
        }
    }
}
