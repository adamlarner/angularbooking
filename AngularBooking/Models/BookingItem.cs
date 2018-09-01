using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AngularBooking.Models
{
    public class BookingItem : IModel
    {
        public int Id { get; set; }
        public int BookingId { get; set; }
        public int Location { get; set; }
        public float AgreedPrice { get; set; }
        public string AgreedPriceName { get; set; }

        // navigation
        public Booking Booking { get; set; }
    }
}
