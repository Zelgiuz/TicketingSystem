using Newtonsoft.Json;
using TicketingSystem.Models;

namespace TicketingSystem.Controllers
{
    public class Ticket
    {
        [JsonProperty("id")]
        public int Id { get; set; }
        [JsonProperty("SectionId")]
        public int SectionId { get; set; }
        public string EventId { get; set; }
        public bool IsSold { get; set; }
        public bool IsReserved { get; set; }
        public int ReservedUser { get; set; }
        public DateTime ReservedUntilDateTime { get; set; }

        public Ticket(Section section, int seatNumber)
        {
            Id = seatNumber;
            SectionId = section.Id;
            EventId = section.EventId;
            IsSold = false;
            IsReserved = false;
            ReservedUser = -1;
            ReservedUntilDateTime = DateTime.MaxValue;
        }
    }
}
