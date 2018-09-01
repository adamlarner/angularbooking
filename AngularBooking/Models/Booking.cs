using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AngularBooking.Models
{
    public class Booking : IModel
    {
        public int Id { get; set; }
        public int ShowingId { get; set; }
        public int CustomerId { get; set; }
        public DateTime BookedDate { get; set; }
        public BookingStatus Status { get; set; }
        public string AdditionalDetails { get; set; }

        // navigation
        public Showing Showing { get; set; }
        public Customer Customer { get; set; }
        public List<BookingItem> BookingItems { get; set; }
    }
}
