using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AngularBooking.Models
{
    public class User : IdentityUser
    {
        // navigation
        public Customer Customer { get; set; }
    }
}
