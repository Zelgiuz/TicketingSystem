using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Cosmos;

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
            var container = database.GetContainer("Tickets");
            var item = new Ticket()
            {
                SectionId = Guid.NewGuid().ToString(),
                EventId = Guid.NewGuid().ToString(),
                IsReserved = false,
                IsSold = false,
                Id = Guid.NewGuid().ToString(),
                Seat = "1A",
                ReservedDateTime = DateTime.MinValue,
            };

            await container.UpsertItemAsync(item);

            List<Ticket> tickets = await container.QueryAsync<Ticket>(x => x.Id == item.Id);


            return new OkObjectResult(tickets[0]);
        }
    }
}
