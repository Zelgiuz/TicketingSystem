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
                if (eventMaker == null || eventMaker.Categories.Count < 1)
                    return new BadRequestObjectResult("Must have a valid event with at least one category");

            }
            catch (Exception ex)
            {
                return new BadRequestObjectResult(ex);
            }
            return new OkObjectResult(result);



        }
    }
}
