using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AngularBooking.Models
{
    public class Event : IModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Image { get; set; }
        public long Duration { get; set; }
        public AgeRatingType AgeRating { get; set; }

        // navigation
        public List<Showing> Showings { get; set; }
    }
}
