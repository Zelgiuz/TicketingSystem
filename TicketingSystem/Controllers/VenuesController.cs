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
        [HttpGet]
        public async Task<IActionResult> CreateAVenue()
        {
            var container = database.GetContainer("Venues");
            var venue = new Venue()
            {
                Id = Guid.NewGuid().ToString(),
                Description = "A Test Venue",
                Name = "Awesome"
            };
            await container.UpsertItemAsync(venue);
            return new OkObjectResult(venue);
        }
    }
}
