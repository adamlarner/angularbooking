using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace AngularBooking.Models.View.Booking
{
    public class CreateBookingViewModel
    {
        [Required]
        public Customer Customer { get; set; }
        [Required]
        public int ShowingId { get; set; }
        [Required]
        public List<CreateBookingItemViewModel> BookingItems { get; set; }
    }
}
