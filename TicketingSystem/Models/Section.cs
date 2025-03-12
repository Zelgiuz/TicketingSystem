using Newtonsoft.Json;

namespace TicketingSystem.Models
{
    public class Section
    {
        [JsonProperty("id")]
        public int Id { get; set; }
        public decimal Price { get; set; }
        [JsonProperty("EventId")]
        public string EventId { get; set; }
        public int SectionCapacity { get; set; }
    }
}
