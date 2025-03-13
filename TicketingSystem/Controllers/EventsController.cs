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
                    return new BadRequestObjectResult("Must have a valid event with at least one category");
                result = eventMaker.CreateEvent();

                var eventContainer = database.GetContainer("Events");
                var ticketContainer = database.GetContainer("Tickets");


                await eventContainer.UpsertItemAsync(result);
                List<Task> tasks = new List<Task>();
                foreach (var section in result.Sections)
                {
                    for (int i = 0; i <= section.SectionCapacity; i++)
                    {
                        Ticket ticket = new Ticket(section, i);
                        await ticketContainer.UpsertItemAsync(ticket);
                    }
                }



            }
            catch (Exception ex)
            {
                return new BadRequestObjectResult(ex);
            }
            return new OkObjectResult(result);



        }
        [HttpGet]
        public async Task<IActionResult> GetEvents([FromQuery] string name)
        {
            var eventContainer = database.GetContainer("Events");
            List<Event> results = await eventContainer.QueryAsync<Event>(x => x.Name == name);
            return new OkObjectResult(results);
        }
    }
}
