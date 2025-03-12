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
            return new OkResult();
        }
    }
}
