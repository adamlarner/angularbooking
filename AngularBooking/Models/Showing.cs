using Microsoft.AspNet.OData.Query;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AngularBooking.Models
{
    public class Showing : IModel
    {
        public int Id { get; set; }
        public int EventId { get; set; }
        public int RoomId { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public int PricingStrategyId { get; set; }

        // navigation
        public Event Event { get; set; }
        public Room Room { get; set; }
        public PricingStrategy PricingStrategy { get; set; }

        [NotExpandable]
        public List<Booking> Bookings { get; set; }

    }
}
