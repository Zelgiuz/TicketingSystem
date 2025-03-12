using Newtonsoft.Json;
using TicketingSystem.Models;

namespace TicketingSystem.Controllers
{
    public class Event
    {
        public Event() { }

        [JsonProperty("id")]
        public string Id { get; set; }
        public string VenueId { get; set; }
        [JsonProperty("Id")]
        public string Name { get; set; }
        public string Description { get; set; }
        public DateTime StartDate { get; set; }
        public List<Section> EventCategories { get; set; } = new List<Section>();

    }
}
