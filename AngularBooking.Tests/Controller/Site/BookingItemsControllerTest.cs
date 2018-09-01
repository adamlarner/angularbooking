using AngularBooking.Controllers.Site;
using AngularBooking.Data;
using AngularBooking.Models;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xunit;

namespace AngularBooking.Tests.Controller.Site
{
    public class BookingItemsControllerTest
    {
        [Fact]
        public void Should_GetBookings()
        {
            // mock UoW and repository data
            Mock<IUnitOfWork> mock = new Mock<IUnitOfWork>();
            mock.Setup(f => f.BookingItems.Get()).Returns(new List<BookingItem>
            {
                new BookingItem { Id = 1, BookingId = 1, Location = 0, AgreedPriceName = "Adult", AgreedPrice = 5.2f },
                new BookingItem { Id = 2, BookingId = 1, Location = 1, AgreedPriceName = "Adult", AgreedPrice = 5.2f },
                new BookingItem { Id = 3, BookingId = 1, Location = 2, AgreedPriceName = "Adult", AgreedPrice = 5.2f },
                new BookingItem { Id = 4, BookingId = 1, Location = 3, AgreedPriceName = "Adult", AgreedPrice = 5.2f },
                new BookingItem { Id = 5, BookingId = 1, Location = 4, AgreedPriceName = "Adult", AgreedPrice = 5.2f }

            }
            .AsQueryable());

            BookingItemsController controller = new BookingItemsController(mock.Object);
            var bookingItems = controller.GetBookingItems();
            Assert.True(bookingItems.Count() == 5);
        }

        [Fact]
        public void Should_GetBookingItem()
        {
            BookingItem testBookingItem = new BookingItem { Id = 1, Location = 0, AgreedPriceName = "Adult", AgreedPrice = 5, BookingId = 1 };

            Mock<IUnitOfWork> mock = new Mock<IUnitOfWork>();
            mock.Setup(f => f.BookingItems.GetById(1)).Returns(testBookingItem);

            BookingItemsController controller = new BookingItemsController(mock.Object);
            var bookingItem = controller.GetBookingItem(1);
            Assert.IsType<OkObjectResult>(bookingItem);
        }

        [Fact]
        public void Should_PutBookingItem()
        {
            BookingItem testBookingItem = new BookingItem { Id = 1 };

            Mock<IUnitOfWork> mock = new Mock<IUnitOfWork>();
            mock.Setup(f => f.BookingItems.Update(testBookingItem)).Returns(true);

            BookingItemsController controller = new BookingItemsController(mock.Object);
            var bookingItems = controller.PutBookingItem(1, testBookingItem);
            Assert.IsType<NoContentResult>(bookingItems);
        }

        [Fact]
        public void ShouldNot_PutBookingItem_ModelStateError()
        {
            BookingItem testBookingItem = new BookingItem { Id = 1 };

            Mock<IUnitOfWork> mock = new Mock<IUnitOfWork>();
            mock.Setup(f => f.BookingItems.Update(testBookingItem)).Returns(true);

            BookingItemsController controller = new BookingItemsController(mock.Object);
            controller.ModelState.AddModelError("TestError", "Error");
            var bookingItems = controller.PutBookingItem(1, testBookingItem);
            Assert.IsType<BadRequestObjectResult>(bookingItems);
        }

        [Fact]
        public void ShouldNot_PutBookingItem_IdMismatch()
        {
            BookingItem testBookingItem = new BookingItem { Id = 1 };

            Mock<IUnitOfWork> mock = new Mock<IUnitOfWork>();
            mock.Setup(f => f.BookingItems.Update(testBookingItem)).Returns(true);

            BookingItemsController controller = new BookingItemsController(mock.Object);
            var bookingItems = controller.PutBookingItem(2, testBookingItem);
            Assert.IsType<BadRequestResult>(bookingItems);
        }

        [Fact]
        public void Should_PostBookingItem()
        {
            BookingItem testBookingItem = new BookingItem { Id = 1, Location = 0, AgreedPriceName = "Adult", AgreedPrice = 5, BookingId = 1 };

            Mock<IUnitOfWork> mock = new Mock<IUnitOfWork>();
            mock.Setup(f => f.BookingItems.Create(testBookingItem)).Returns(true);

            BookingItemsController controller = new BookingItemsController(mock.Object);
            var bookingItems = controller.PostBookingItem(testBookingItem);
            Assert.IsType<CreatedAtActionResult>(bookingItems);

        }

        [Fact]
        public void ShouldNot_PostBookingItem_ModelStateError()
        {
            BookingItem testBookingItem = new BookingItem { Id = 1, Location = 0, AgreedPriceName = "Adult", AgreedPrice = 5, BookingId = 1 };

            Mock<IUnitOfWork> mock = new Mock<IUnitOfWork>();
            mock.Setup(f => f.BookingItems.Create(testBookingItem)).Returns(true);
            mock.Setup(f => f.BookingItems.GetById(1)).Returns(testBookingItem);

            BookingItemsController controller = new BookingItemsController(mock.Object);
            controller.ModelState.AddModelError("TestError", "Error");
            var bookingItems = controller.PostBookingItem(testBookingItem);
            Assert.IsType<BadRequestObjectResult>(bookingItems);
        }

        [Fact]
        public void Should_DeleteBookingItem()
        {
            BookingItem testBookingItem = new BookingItem { Id = 1 };

            Mock<IUnitOfWork> mock = new Mock<IUnitOfWork>();
            mock.Setup(f => f.BookingItems.GetById(1)).Returns(testBookingItem);
            mock.Setup(f => f.BookingItems.Delete(testBookingItem)).Returns(true);

            BookingItemsController controller = new BookingItemsController(mock.Object);
            var result = controller.DeleteBookingItem(1);
            Assert.IsType<OkObjectResult>(result);
        }

        [Fact]
        public void ShouldNot_DeleteBookingItem_ModelStateError()
        {
            BookingItem testBookingItem = new BookingItem { Id = 1 };

            Mock<IUnitOfWork> mock = new Mock<IUnitOfWork>();
            mock.Setup(f => f.BookingItems.Delete(testBookingItem)).Returns(true);

            BookingItemsController controller = new BookingItemsController(mock.Object);
            controller.ModelState.AddModelError("TestError", "Error");
            var result = controller.DeleteBookingItem(1);
            Assert.IsType<BadRequestObjectResult>(result);
        }

        [Fact]
        public void ShouldNot_DeleteBookingItem_NotFound()
        {
            BookingItem testBookingItem = new BookingItem { Id = 10 };

            Mock<IUnitOfWork> mock = new Mock<IUnitOfWork>();
            mock.Setup(f => f.BookingItems.GetById(10)).Returns((BookingItem)null);

            BookingItemsController controller = new BookingItemsController(mock.Object);
            var result = controller.DeleteBookingItem(10);
            Assert.IsType<NotFoundResult>(result);
        }

    }
}
