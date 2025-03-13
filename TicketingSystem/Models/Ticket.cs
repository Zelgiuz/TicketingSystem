using Newtonsoft.Json;
using TicketingSystem.Extensions;
using TicketingSystem.Models;

namespace TicketingSystem.Controllers
{
    public class Ticket
    {
        [JsonProperty("id")]
        public string Id { get; set; }
        public int SeatNumber { get; set; }
        public string SectionId { get; set; }
        public string EventId { get; set; }
        public bool IsSold { get; set; }
        public bool IsReserved { get; set; }
        public int ReservedUser { get; set; }
        public string ReservedUntilDateTime { get; set; }

        public Ticket(Section section, int seatNumber)
        {
            Id = Guid.NewGuid().ToString();
            SeatNumber = seatNumber;
            SectionId = section.Id;
            EventId = section.EventId;
            IsSold = false;
            IsReserved = false;
            ReservedUser = -1;
            ReservedUntilDateTime = DateTime.MinValue.ToIso8601();
        }
    }
}
