namespace TicketingSystem.Models
{
    public class SectionModel
    {
        public string Name { get; set; }
        public int Capacity { get; set; }
        public decimal Price { get; set; }

        public Section MakeSection(string eventId)
        {
            return new Section()
            {
                SectionCapacity = Capacity,
                SectionName = Name,
                EventId = eventId,
                Price = Price,
                Id = Guid.NewGuid().ToString()
            };

        }
    }
}
