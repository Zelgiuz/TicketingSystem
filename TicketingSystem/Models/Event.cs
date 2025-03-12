using TicketingSystem.Models;

namespace TicketingSystem.Controllers
{
    public class Event
    {
        public Event() { }

        public Guid EventId { get; set; }
        public Guid VenueId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string StartDate { get; set; }
        public List<EventCategory> EventCategories { get; set; } = new List<EventCategory>();

    }
}
