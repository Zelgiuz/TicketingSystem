using TicketingSystem.Controllers;

namespace TicketingSystem.Models
{
    public class EventModel
    {
        public string Name { get; set; }
        public DateTime StartDate { get; set; }
        public string VenueId { get; set; }
        public string Description { get; set; }

        public List<SectionModel> Categories { get; set; }


        public Event CreateEvent()
        {
            Event shindig = new Event();
            shindig.Description = Description;
            shindig.Id = Guid.NewGuid().ToString();
            shindig.StartDate = StartDate;
            shindig.VenueId = VenueId;
            shindig.Name = Name;

            List<Section> categories = new List<Section>();
            foreach (var category in Categories)
            {
                categories.Add(category.MakeSection(shindig.Id));
            }
            shindig.EventCategories = categories;
            return shindig;

        }

    }
}
