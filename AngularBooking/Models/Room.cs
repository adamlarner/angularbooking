using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AngularBooking.Models
{
    public class Room : IModel
    {
        public int Id { get; set; }
        public int VenueId { get; set; }
        public string Name { get; set; }
        public string Description { get;set; }
        public int Rows { get; set; }
        public int Columns { get; set; }
        public string Isles { get; set; } // JSON

        // navigation
        public Venue Venue { get; set; }
        public List<Showing> Showings { get; set; }
    }
}
