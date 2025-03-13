namespace TicketingSystem.Models
{
    public class SectionModel
    {
        public string Name { get; set; }
        public int Capacity { get; set; }
        public decimal Price { get; set; }

        public Section MakeSection(string eventId, string sectionNumber)
        {
            return new Section(this, eventId, sectionNumber);
        }
    }
}
