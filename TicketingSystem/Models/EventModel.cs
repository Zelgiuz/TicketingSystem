using TicketingSystem.Controllers;

namespace TicketingSystem.Models
{
    public class EventModel
    {
        public string Name { get; set; }
        public DateTime StartDate { get; set; }
        public string VenueId { get; set; }
        public string Description { get; set; }

        public List<SectionModel> Sections { get; set; }


        public Event CreateEvent()
        {
            Event shindig = new Event();
            shindig.Description = Description;
            shindig.Id = Guid.NewGuid().ToString();
            shindig.StartDate = StartDate;
            shindig.VenueId = VenueId;
            shindig.Name = Name;

            List<Section> sections = new List<Section>();
            int i = 1;
            foreach (var section in Sections)
            {
                sections.Add(section.MakeSection(shindig.Id, i));
                i++;
            }
            shindig.Sections = sections;
            return shindig;

        }

    }
}
