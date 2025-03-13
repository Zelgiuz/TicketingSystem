﻿using Newtonsoft.Json;
using TicketingSystem.Models;

namespace TicketingSystem.Controllers
{
    public class Event
    {
        public Event() { }

        [JsonProperty("id")]
        public string Id { get; set; }
        public string VenueId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public DateTime StartDate { get; set; }
        public List<Section> Sections { get; set; } = new List<Section>();

    }
}
