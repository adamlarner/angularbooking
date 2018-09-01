using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AngularBooking.Services.Email
{
    public interface IEmailService
    {
        bool SendEmail(string to, string subject, string content, string from = null);
    }
}
