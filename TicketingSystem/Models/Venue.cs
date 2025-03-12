using Newtonsoft.Json;
using System.Text.Json;

namespace TicketingSystem.Models
{
    public class Venue
    {
        [JsonProperty("id")]
        public string Id { get; set; }
        [JsonProperty("Name")]
        public string Name { get; set; }
        public string Description { get; set; }
        public Venue() { }

    }
}
