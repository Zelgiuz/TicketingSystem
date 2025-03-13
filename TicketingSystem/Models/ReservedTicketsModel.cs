using TicketingSystem.Controllers;

namespace TicketingSystem.Models
{
    public class ReservedTicketsModel
    {
        public List<Ticket> Tickets { get; set; }
        public decimal price { get; set; }

        public ReservedTicketsModel()
        {
        }
        public ReservedTicketsModel(List<Ticket> tickets, decimal price)
        {
            Tickets = tickets;
            this.price = price;
        }
    }
}
