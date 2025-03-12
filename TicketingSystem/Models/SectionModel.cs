namespace TicketingSystem.Models
{
    public class SectionModel
    {
        public string Name { get; set; }
        public int Capacity { get; set; }
        public decimal Price { get; set; }

        public Section MakeSection(string eventId, int sectionNumber)
        {
            return new Section()
            {
                SectionCapacity = Capacity,
                EventId = eventId,
                Price = Price,
                Id = sectionNumber
            };

        }
    }
}
