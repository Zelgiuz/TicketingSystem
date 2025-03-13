using Newtonsoft.Json;

namespace TicketingSystem.Models
{
    public class Section
    {
        [JsonProperty("id")]
        public string Id { get; set; }
        public decimal Price { get; set; }
        [JsonProperty("EventId")]
        public string EventId { get; set; }
        public int SectionCapacity { get; set; }

        public Section()
        {
        }

        public Section(SectionModel model, string eventId, string sectionId)
        {
            Id = sectionId;
            Price = model.Price;
            EventId = eventId;
            SectionCapacity = model.Capacity;

        }
    }
}
