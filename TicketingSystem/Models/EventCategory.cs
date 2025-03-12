namespace TicketingSystem.Models
{
    public class EventCategory
    {
        public decimal Price { get; set; }
        public Guid EventID { get; set; }
        public Guid EventCategoryID { get; set; }
        public string EventCategoryName { get; set; } = "";
        public string EventCategoryDescription { get; set; } = "";
        public int EventCategoryCapacity { get; set; }
    }
}
