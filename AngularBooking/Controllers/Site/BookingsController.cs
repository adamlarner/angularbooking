using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using AngularBooking.Data;
using AngularBooking.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;
using Microsoft.AspNet.OData;
using AngularBooking.Models.View.Booking;
using AngularBooking.Services.Email;
using System.Text;

namespace AngularBooking.Controllers.Site
{
    [Route("api/[controller]")]
    [AutoValidateAntiforgeryToken]
    [ApiController]
    public class BookingsController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;
        private UserManager<User> _userManager;
        private IEmailService _emailService;

        public BookingsController(IUnitOfWork unitOfWork, UserManager<User> userManager, IEmailService emailService)
        {
            _unitOfWork = unitOfWork;
            _userManager = userManager;
            _emailService = emailService;
        }

        // GET: api/Bookings
        [EnableQuery()]
        [Authorize]
        [HttpGet]
        public IQueryable<Booking> GetBookings()
        {
            // get user email from claim
            Claim userClaim = HttpContext.User.Claims.SingleOrDefault(f => f.Type == "sub");

            // problem retrieving claim, so return empty
            if (userClaim == null)
                return Enumerable.Empty<Booking>().AsQueryable();

            // check role and filter list based on permitted access
            User user = _userManager.FindByNameAsync(userClaim.Value).Result;

            // problem retrieving user, so return empty
            if (user == null)
                return Enumerable.Empty<Booking>().AsQueryable();

            // if user is in admin role, return entire set
            if (_userManager.IsInRoleAsync(user, "admin").Result)
            {
                return _unitOfWork.Bookings.Get();
            }

            // get customer id for user (improve performance by querying only id)
            int customerId = _unitOfWork.Customers.Get().Where(f => f.UserId == user.Id).Select(f => f.Id).FirstOrDefault();

            // problem retrieving customer, so return empty
            if (customerId == 0)
                return Enumerable.Empty<Booking>().AsQueryable();

            // query only bookings that belong to customer
            IQueryable<Booking> filtered = _unitOfWork.Bookings.Get().Where(f => f.CustomerId == customerId);

            return filtered;
        }

        // GET: api/Bookings/5
        [Authorize]
        [HttpGet("{id}")]
        public IActionResult GetBooking([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            // get user email from claim
            Claim userClaim = HttpContext.User.Claims.SingleOrDefault(f => f.Type == "sub");

            // problem retrieving claim, so return empty
            if (userClaim == null)
                return BadRequest();

            // check roll, and confirm user is permitted
            User user = _userManager.FindByNameAsync(userClaim.Value).Result;

            if (user == null)
            {
                return Unauthorized();
            }

            // determine whether user is admin
            var isAdmin = _userManager.IsInRoleAsync(user, "admin").Result;

            // get customer id for user (improve performance by querying only id)
            int customerId = _unitOfWork.Customers.Get().Where(f => f.UserId == user.Id).Select(f => f.Id).FirstOrDefault();

            // problem retrieving customer (skip if admin)
            if (customerId == 0 && !isAdmin)
                return BadRequest();

            Booking booking = null;

            if (isAdmin)
            {
                // permitted, so query all ids
                booking = _unitOfWork.Bookings.Get().Where(f => f.Id == id).FirstOrDefault();
            }
            else
            {
                // add condition for matching customer id
                booking = _unitOfWork.Bookings.Get().Where(f => f.Id == id && f.CustomerId == customerId).FirstOrDefault();
            }

            if (booking == null)
            {
                return NotFound();
            }

            return Ok(booking);
        }

        // PUT: api/Bookings/5
        [Authorize(Roles = "admin")]
        [HttpPut("{id}")]
        public IActionResult PutBooking([FromRoute] int id, [FromBody] Booking booking)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != booking.Id)
            {
                return BadRequest();
            }

            try
            {
                _unitOfWork.Bookings.Update(booking);
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!BookingExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/Bookings
        [Authorize(Roles = "admin")]
        [HttpPost]
        public IActionResult PostBooking([FromBody] Booking booking)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            _unitOfWork.Bookings.Create(booking);

            return CreatedAtAction("GetBooking", new { id = booking.Id }, booking);
        }

        // DELETE: api/Bookings/5
        [Authorize(Roles = "admin")]
        [HttpDelete("{id}")]
        public IActionResult DeleteBooking([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var booking = _unitOfWork.Bookings.GetById(id);
            if (booking == null)
            {
                return NotFound();
            }

            _unitOfWork.Bookings.Delete(booking);

            return Ok(booking);
        }

        // create booking
        [HttpPost("create")]
        public IActionResult CreateBooking([FromBody] CreateBookingViewModel model)
        {
            // check to see if authenticated user has made request
            Claim userClaim = HttpContext.User.Claims.SingleOrDefault(f => f.Type == "sub");

            User user = null;
            Customer customer = null;

            if (userClaim != null)
            {
                user = _userManager.FindByNameAsync(userClaim.Value).Result;
                if (user != null)
                {
                    customer = _unitOfWork.Customers.Get().SingleOrDefault(f => f.UserId == user.Id);
                }
            }

            // create/update customer details with those provided in model
            bool customerIsNew = false;
            if (customer == null)
            {
                // set email, since no user is attributed to account (unique email check not in place, since it's not linked to account)
                customerIsNew = true;
                customer = new Customer { Id = 0, ContactEmail = model.Customer.ContactEmail };
            }

            bool customerValid = false;

            customer.Address1 = model.Customer.Address1;
            customer.Address2 = model.Customer.Address2;
            customer.Address3 = model.Customer.Address3;
            customer.Address4 = model.Customer.Address4;
            customer.Address5 = model.Customer.Address5;
            customer.ContactPhone = model.Customer.ContactPhone;
            customer.FirstName = model.Customer.FirstName;
            customer.LastName = model.Customer.LastName;
            if (customerIsNew)
                customerValid = _unitOfWork.Customers.Create(customer);
            else
                customerValid = _unitOfWork.Customers.Update(customer);

            // customer cannot be created, so return bad request
            if (!customerValid)
                return BadRequest();

            // use showing id to get showing details (incl pricing strategy and items)
            Showing showing = _unitOfWork.Showings.Get().
                Include(f => f.Event).
                Include(f => f.Room).ThenInclude(f => f.Venue).
                Include(f => f.PricingStrategy).
                ThenInclude(f => f.PricingStrategyItems).
                SingleOrDefault(f => f.Id == model.ShowingId);

            // no showing found, so attempt to delete customer if new, and return bad request
            if (showing == null)
            {
                if (customerIsNew)
                    _unitOfWork.Customers.Delete(customer);

                return BadRequest();
            }

            DateTime bookedDate = DateTime.UtcNow;

            // build booking and items, and attempt creation
            Booking newBooking = new Booking
            {
                BookedDate = bookedDate,
                Status = BookingStatus.PaymentComplete,
                AdditionalDetails = "",
                ShowingId = model.ShowingId,
                CustomerId = customer.Id,
                BookingItems = new List<BookingItem>()
            };

            foreach (var item in model.BookingItems)
            {
                // get the pricing strategy item if it exists, and if not cancel and return bad request
                PricingStrategyItem pricingItem = showing.PricingStrategy.PricingStrategyItems.SingleOrDefault(f => f.Id == item.PricingStrategyItemId);

                if (pricingItem == null)
                {
                    if (customerIsNew)
                        _unitOfWork.Customers.Delete(customer);

                    return BadRequest();
                }

                newBooking.BookingItems.Add(new BookingItem
                {
                    AgreedPrice = pricingItem.Price,
                    AgreedPriceName = pricingItem.Name,
                    Location = item.Location
                });
            }

            // attempt booking creation (all or nothing, since a pre-booking reservation mechanism is not implemented)
            bool bookingCompleted = _unitOfWork.Bookings.Create(newBooking);

            // simple "yay or nay" response (this can further improved by building better error checking/scrapping 'UoW' wrapper)
            if (!bookingCompleted)
                return BadRequest();

            // send email confirming details of the booking
            StringBuilder emailContent = new StringBuilder();
            emailContent.Append($"Thank you for booking with us! The details of your booking can be found below:\n\n");

            emailContent.Append($"Booking Date: {bookedDate.ToLongDateString()}\n\n");

            emailContent.Append($"Event: {showing.Event.Name}\n");
            emailContent.Append($"Venue: {showing.Room.Venue.Name}\n");
            emailContent.Append($"Room: {showing.Room.Name}\n");
            // use local time, since there's no user/venue timezone data to offset UTC time against
            emailContent.Append($"Start Time: {showing.StartTime.ToLocalTime().ToString()}\n\n");

            emailContent.Append("Admissions\n");
            emailContent.Append("----------------------------------------------------\n\n");
            emailContent.Append("Name\t\tPrice\t\tSeat\n");
            emailContent.Append("----------------------------------------------------\n\n");

            foreach (BookingItem item in newBooking.BookingItems)
            {
                // calcuate location string using location row and column (calculate first using room data)
                int roomColumns = showing.Room.Columns;

                int row = item.Location / roomColumns;
                int column = item.Location % roomColumns;
                string locationName = calculateLocationName(row, column);

                emailContent.Append($"{item.AgreedPriceName}\t\t£{item.AgreedPrice}\t\t{locationName}\n");
            }
            emailContent.Append("----------------------------------------------------\n");
            emailContent.Append($"Total\t\t£{newBooking.BookingItems.Sum(f => f.AgreedPrice).ToString("F2")}");

            _emailService.SendEmail(customer.ContactEmail, "Booking Details", emailContent.ToString());

            return Ok();
        }

        // similar to update, except booking is marked as cancelled and booking items are deleted instead (a note can be left within "additional details", regarding previous booking)
        [Authorize]
        [HttpPost("cancel")]
        public IActionResult CancelBooking([FromBody] int id)
        {
            // get user email from claim
            Claim userClaim = HttpContext.User.Claims.SingleOrDefault(f => f.Type == "sub");

            // problem retrieving claim, so return empty
            if (userClaim == null)
                return BadRequest();

            // check roll, and confirm user is permitted
            User user = _userManager.FindByNameAsync(userClaim.Value).Result;

            if (user == null)
            {
                return Unauthorized();
            }

            // determine whether user is admin
            var isAdmin = _userManager.IsInRoleAsync(user, "admin").Result;

            // get customer id for user (improve performance by querying only id)
            int customerId = _unitOfWork.Customers.Get().Where(f => f.UserId == user.Id).Select(f => f.Id).FirstOrDefault();

            // problem retrieving customer (skip if admin)
            if (customerId == 0 && !isAdmin)
                return BadRequest();

            Booking booking = null;

            if (isAdmin)
            {
                // permitted, so query all ids
                booking = _unitOfWork.Bookings.Get().Include(f => f.BookingItems).Where(f => f.Id == id).FirstOrDefault();
            }
            else
            {
                // add condition for matching customer id
                booking = _unitOfWork.Bookings.Get().Include(f => f.BookingItems).Where(f => f.Id == id && f.CustomerId == customerId).FirstOrDefault();
            }

            if (booking == null)
            {
                return NotFound();
            }

            // delete booking items, and update booking status
            for (int i = booking.BookingItems.Count - 1; i > -1; i--)
            {
                _unitOfWork.BookingItems.Delete(booking.BookingItems[i]);
            }

            booking.Status = BookingStatus.Cancelled;
            _unitOfWork.Bookings.Update(booking);

            return Ok();
        }

        [Authorize(Roles = "admin")]
        private bool BookingExists(int id)
        {
            return _unitOfWork.Bookings.Exists(new Booking { Id = id });
        }

        // utility methods
        private string calculateLocationName(int row, int column)
        {
            // columns are hexavigesimal, rows are base 10
            float hexvig = column + 1;
            var hexvigOutput = "";
            while (Math.Floor(hexvig) > 0)
            {
                // needed to ensure mod output is zero based
                hexvig -= 1;
                // character represented by current position ('A' dec '65')
                hexvigOutput = ((char)(65 + (hexvig % 26))) + hexvigOutput;
                // move to next position
                hexvig /= 26;
            }

            return $"col: {hexvigOutput} - row: {row}";
        }

    }
}