namespace TicketingSystem.Models
{
    public class TicketsToChangeModel
    {
        public List<TicketNumbers> Tickets { get; set; }
        public int UserId { get; set; }
    }

    public class TicketNumbers
    {
        public int SeatNumber { get; set; }
        public string SectionId { get; set; }
    }
}
