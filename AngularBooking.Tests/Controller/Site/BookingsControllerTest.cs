using AngularBooking.Controllers.Site;
using AngularBooking.Data;
using AngularBooking.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace AngularBooking.Tests.Controller.Site
{
    public class BookingsControllerTest
    {
        [Fact]
        public void Should_GetBookings()
        {
            // mock UoW and repository data
            Mock<IUnitOfWork> mock = new Mock<IUnitOfWork>();
            mock.Setup(f => f.Bookings.Get()).Returns(new List<Booking>
            {
                new Booking { Id = 1, CustomerId = 1, ShowingId = 1, Status = BookingStatus.PaymentPending, BookedDate = DateTime.Parse("01-01-2020 12:00"), BookingItems = new List<BookingItem>() },
                new Booking { Id = 2, CustomerId = 1, ShowingId = 2, Status = BookingStatus.PaymentPending, BookedDate = DateTime.Parse("01-01-2020 12:01"), BookingItems = new List<BookingItem>() },
                new Booking { Id = 3, CustomerId = 2, ShowingId = 3, Status = BookingStatus.PaymentPending, BookedDate = DateTime.Parse("01-01-2020 12:02"), BookingItems = new List<BookingItem>() },
                new Booking { Id = 4, CustomerId = 3, ShowingId = 4, Status = BookingStatus.PaymentPending, BookedDate = DateTime.Parse("01-01-2020 12:03"), BookingItems = new List<BookingItem>() },
                new Booking { Id = 5, CustomerId = 4, ShowingId = 5, Status = BookingStatus.PaymentComplete, BookedDate = DateTime.Parse("01-01-2020 12:04"), BookingItems = new List<BookingItem>() }
            }
            .AsQueryable());
            mock.Setup(f => f.Customers.Get()).Returns(new List<Customer>
            {
                new Customer { Id = 1, UserId = "test" }
            }.AsQueryable());

            // mock UserManager
            var userStore = new Mock<IUserStore<User>>();
            Mock<UserManager<User>> mockUserManager = new Mock<UserManager<User>>(userStore.Object, null, null, null, null, null, null, null, null);
            mockUserManager.Setup(f => f.IsInRoleAsync(It.IsAny<User>(), "admin")).Returns(Task.FromResult(false));
            mockUserManager.Setup(f => f.FindByNameAsync(It.IsAny<string>())).Returns(Task.FromResult(new User { Id = "test" }));

            var controller = new BookingsController(mock.Object, mockUserManager.Object, null);
            var userClaimsIdentity = new ClaimsPrincipal();
            userClaimsIdentity.AddIdentity(new ClaimsIdentity(new[] { new Claim("sub", "test@test.com") }));
            controller.ControllerContext.HttpContext = new DefaultHttpContext() { User = userClaimsIdentity };
            var bookings = controller.GetBookings();
            Assert.True(bookings.Count() == 2);
        }

        [Fact]
        public void Should_GetBookings_AsAdmin()
        {
            // mock UoW and repository data
            Mock<IUnitOfWork> mockUOW = new Mock<IUnitOfWork>();
            mockUOW.Setup(f => f.Bookings.Get()).Returns(new List<Booking>
            {
                new Booking { Id = 1, CustomerId = 1, ShowingId = 1, Status = BookingStatus.PaymentPending, BookedDate = DateTime.Parse("01-01-2020 12:00"), BookingItems = new List<BookingItem>() },
                new Booking { Id = 2, CustomerId = 1, ShowingId = 2, Status = BookingStatus.PaymentPending, BookedDate = DateTime.Parse("01-01-2020 12:01"), BookingItems = new List<BookingItem>() },
                new Booking { Id = 3, CustomerId = 2, ShowingId = 3, Status = BookingStatus.PaymentPending, BookedDate = DateTime.Parse("01-01-2020 12:02"), BookingItems = new List<BookingItem>() },
                new Booking { Id = 4, CustomerId = 3, ShowingId = 4, Status = BookingStatus.PaymentPending, BookedDate = DateTime.Parse("01-01-2020 12:03"), BookingItems = new List<BookingItem>() },
                new Booking { Id = 5, CustomerId = 4, ShowingId = 5, Status = BookingStatus.PaymentComplete, BookedDate = DateTime.Parse("01-01-2020 12:04"), BookingItems = new List<BookingItem>() }
            }
            .AsQueryable());
            mockUOW.Setup(f => f.Customers.Get()).Returns(new List<Customer>
            {
                new Customer { Id = 1, UserId = "test" }
            }.AsQueryable());

            // mock UserManager
            var userStore = new Mock<IUserStore<User>>();
            Mock<UserManager<User>> mockUserManager = new Mock<UserManager<User>>(userStore.Object, null, null, null, null, null, null, null, null);
            mockUserManager.Setup(f => f.IsInRoleAsync(It.IsAny<User>(), "admin")).Returns(Task.FromResult(true));
            mockUserManager.Setup(f => f.FindByNameAsync(It.IsAny<string>())).Returns(Task.FromResult(new User { Id = "test" }));

            BookingsController controller = new BookingsController(mockUOW.Object, mockUserManager.Object, null);
            var userClaimsIdentity = new ClaimsPrincipal();
            userClaimsIdentity.AddIdentity(new ClaimsIdentity(new[] { new Claim("sub", "test@test.com") }));
            controller.ControllerContext.HttpContext = new DefaultHttpContext() { User = userClaimsIdentity };
            var bookings = controller.GetBookings();
            Assert.True(bookings.Count() == 5);
        }

        [Fact]
        public void Should_GetBooking()
        {
            Mock<IUnitOfWork> mockUOW = new Mock<IUnitOfWork>();
            mockUOW.Setup(f => f.Bookings.Get()).Returns(new List<Booking>
            {
                new Booking { Id = 1, CustomerId = 1, ShowingId = 1, Status = BookingStatus.PaymentPending, BookedDate = DateTime.Parse("01-01-2020 12:00"), BookingItems = new List<BookingItem>() }
            }
            .AsQueryable());
            mockUOW.Setup(f => f.Customers.Get()).Returns(new List<Customer>
            {
                new Customer { Id = 1, UserId = "test" }
            }.AsQueryable());

            // mock UserManager
            var userStore = new Mock<IUserStore<User>>();
            Mock<UserManager<User>> mockUserManager = new Mock<UserManager<User>>(userStore.Object, null, null, null, null, null, null, null, null);
            mockUserManager.Setup(f => f.IsInRoleAsync(It.IsAny<User>(), "admin")).Returns(Task.FromResult(true));
            mockUserManager.Setup(f => f.FindByNameAsync(It.IsAny<string>())).Returns(Task.FromResult(new User { Id = "test" }));

            BookingsController controller = new BookingsController(mockUOW.Object, mockUserManager.Object, null);
            var userClaimsIdentity = new ClaimsPrincipal();
            userClaimsIdentity.AddIdentity(new ClaimsIdentity(new[] { new Claim("sub", "test@test.com") }));
            controller.ControllerContext.HttpContext = new DefaultHttpContext() { User = userClaimsIdentity };
            var booking = controller.GetBooking(1);
            Assert.IsType<OkObjectResult>(booking);
        }

        [Fact]
        public void Should_PutBooking()
        {
            Booking testBooking = new Booking { Id = 1 };

            Mock<IUnitOfWork> mock = new Mock<IUnitOfWork>();
            mock.Setup(f => f.Bookings.Update(testBooking)).Returns(true);

            BookingsController controller = new BookingsController(mock.Object, null, null);
            var bookings = controller.PutBooking(1, testBooking);
            Assert.IsType<NoContentResult>(bookings);
        }

        [Fact]
        public void ShouldNot_PutBooking_ModelStateError()
        {
            Booking testBooking = new Booking { Id = 1 };

            Mock<IUnitOfWork> mock = new Mock<IUnitOfWork>();
            mock.Setup(f => f.Bookings.Update(testBooking)).Returns(true);

            BookingsController controller = new BookingsController(mock.Object, null, null);
            controller.ModelState.AddModelError("TestError", "Error");
            var bookings = controller.PutBooking(1, testBooking);
            Assert.IsType<BadRequestObjectResult>(bookings);
        }

        [Fact]
        public void ShouldNot_PutBooking_IdMismatch()
        {
            Booking testBooking = new Booking { Id = 1 };

            Mock<IUnitOfWork> mock = new Mock<IUnitOfWork>();
            mock.Setup(f => f.Bookings.Update(testBooking)).Returns(true);

            BookingsController controller = new BookingsController(mock.Object, null, null);
            var bookings = controller.PutBooking(2, testBooking);
            Assert.IsType<BadRequestResult>(bookings);
        }

        [Fact]
        public void Should_PostBooking()
        {
            Booking testBooking = new Booking { CustomerId = 1, ShowingId = 1, Status = BookingStatus.PaymentPending, BookedDate = DateTime.Now, BookingItems = new List<BookingItem>() };

            Mock<IUnitOfWork> mock = new Mock<IUnitOfWork>();
            mock.Setup(f => f.Bookings.Create(testBooking)).Returns(true);

            BookingsController controller = new BookingsController(mock.Object, null, null);
            var bookings = controller.PostBooking(testBooking);
            Assert.IsType<CreatedAtActionResult>(bookings);

        }

        [Fact]
        public void ShouldNot_PostBooking_ModelStateError()
        {
            Booking testBooking = new Booking { CustomerId = 1, ShowingId = 1, Status = BookingStatus.PaymentComplete, BookedDate = DateTime.Now, BookingItems = new List<BookingItem>() };

            Mock<IUnitOfWork> mock = new Mock<IUnitOfWork>();
            mock.Setup(f => f.Bookings.Create(testBooking)).Returns(true);
            mock.Setup(f => f.Bookings.GetById(1)).Returns(testBooking);

            BookingsController controller = new BookingsController(mock.Object, null, null);
            controller.ModelState.AddModelError("TestError", "Error");
            var bookings = controller.PostBooking(testBooking);
            Assert.IsType<BadRequestObjectResult>(bookings);
        }

        [Fact]
        public void Should_DeleteBooking()
        {
            Booking testBooking = new Booking { Id = 1 };

            Mock<IUnitOfWork> mock = new Mock<IUnitOfWork>();
            mock.Setup(f => f.Bookings.GetById(1)).Returns(testBooking);
            mock.Setup(f => f.Bookings.Delete(testBooking)).Returns(true);

            BookingsController controller = new BookingsController(mock.Object, null, null);
            var result = controller.DeleteBooking(1);
            Assert.IsType<OkObjectResult>(result);
        }

        [Fact]
        public void ShouldNot_DeleteBooking_ModelStateError()
        {
            Booking testBooking = new Booking { Id = 1 };

            Mock<IUnitOfWork> mock = new Mock<IUnitOfWork>();
            mock.Setup(f => f.Bookings.Delete(testBooking)).Returns(true);

            BookingsController controller = new BookingsController(mock.Object, null, null);
            controller.ModelState.AddModelError("TestError", "Error");
            var result = controller.DeleteBooking(1);
            Assert.IsType<BadRequestObjectResult>(result);
        }

        [Fact]
        public void ShouldNot_DeleteBooking_NotFound()
        {
            Booking testBooking = new Booking { Id = 10 };

            Mock<IUnitOfWork> mock = new Mock<IUnitOfWork>();
            mock.Setup(f => f.Bookings.GetById(10)).Returns((Booking)null);

            BookingsController controller = new BookingsController(mock.Object, null, null);
            var result = controller.DeleteBooking(10);
            Assert.IsType<NotFoundResult>(result);
        }

    }
}
