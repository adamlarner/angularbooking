using AngularBooking.Services.Email;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Xunit;

namespace AngularBooking.Tests.Services.Email
{
    public class BasicEmailServiceTest
    {
        [Fact]
        public void Should_SendTestEmail()
        {
            // create email directory

            string emailDirectory = Path.Combine(Environment.CurrentDirectory, "test_email");
            if(Directory.Exists(emailDirectory))
                Directory.Delete(emailDirectory, true);

            Directory.CreateDirectory(emailDirectory);

            // setup email service and send test 'email'
            BasicEmailService emailService = new BasicEmailService(emailDirectory, ""); //application root
            string data = Guid.NewGuid().ToString();
            emailService.SendEmail("test@solowing.co.uk", "none", data);

            // check 'email' exists
            Assert.Single(Directory.GetFiles(emailDirectory));
        }
    }
}
