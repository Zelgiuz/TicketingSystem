using Newtonsoft.Json;

namespace TicketingSystem.Controllers
{
    public class Ticket
    {
        [JsonProperty("id")]
        public string Id { get; set; }
        [JsonProperty("SectionId")]
        public string SectionId { get; set; }
        public string EventId { get; set; }
        public string Seat { get; set; } = string.Empty;
        public bool IsSold { get; set; }
        public bool IsReserved { get; set; }
        public DateTime ReservedDateTime { get; set; }
    }
}
