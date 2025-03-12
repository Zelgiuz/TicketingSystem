namespace TicketingSystem.Controllers
{
    public class Ticket
    {
        public Guid TicketID { get; set; }
        public Guid EventCategoryID { get; set; }
        public Guid EventID { get; set; }
        public string Seat { get; set; } = string.Empty;
        public bool IsSold { get; set; }
        public bool IsReserved { get; set; }
        public DateTime ReservedDateTime { get; set; }
    }
}
