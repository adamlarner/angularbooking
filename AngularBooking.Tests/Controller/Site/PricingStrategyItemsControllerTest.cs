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
    public class PricingStrategyItemsControllerTest
    {
        [Fact]
        public void Should_GetPricingStrategyItems()
        {
            // mock UoW and repository data
            Mock<IUnitOfWork> mock = new Mock<IUnitOfWork>();
            mock.Setup(f => f.PricingStrategyItems.Get()).Returns(new List<PricingStrategyItem>
            {
                new PricingStrategyItem { Id = 1, Name = "Test1", Description = "Test", Price = 5, PricingStrategyId = 1 },
                new PricingStrategyItem { Id = 2, Name = "Test2", Description = "Test", Price = 6, PricingStrategyId = 1 },
                new PricingStrategyItem { Id = 3, Name = "Test3", Description = "Test", Price = 7, PricingStrategyId = 1 },
                new PricingStrategyItem { Id = 4, Name = "Test4", Description = "Test", Price = 8, PricingStrategyId = 1 },
                new PricingStrategyItem { Id = 5, Name = "Test5", Description = "Test", Price = 9, PricingStrategyId = 1 },
            }
            .AsQueryable());

            PricingStrategyItemsController controller = new PricingStrategyItemsController(mock.Object);
            var pricingStrategyItems = controller.GetPricingStrategyItems();
            Assert.True(pricingStrategyItems.Count() == 5);
        }

        [Fact]
        public void Should_GetPricingStrategyItem()
        {
            PricingStrategyItem testPricingStrategyItem = new PricingStrategyItem { Id = 1, Name = "Test1", Description = "Test", Price = 5, PricingStrategyId = 1 };

            Mock<IUnitOfWork> mock = new Mock<IUnitOfWork>();
            mock.Setup(f => f.PricingStrategyItems.GetById(1)).Returns(testPricingStrategyItem);

            PricingStrategyItemsController controller = new PricingStrategyItemsController(mock.Object);
            var pricingStrategyItem = controller.GetPricingStrategyItem(1);
            Assert.IsType<OkObjectResult>(pricingStrategyItem);
        }

        [Fact]
        public void Should_PutPricingStrategyItem()
        {
            PricingStrategyItem testPricingStrategyItem = new PricingStrategyItem { Id = 1 };

            Mock<IUnitOfWork> mock = new Mock<IUnitOfWork>();
            mock.Setup(f => f.PricingStrategyItems.Update(testPricingStrategyItem)).Returns(true);

            PricingStrategyItemsController controller = new PricingStrategyItemsController(mock.Object);
            var pricingStrategyItems = controller.PutPricingStrategyItem(1, testPricingStrategyItem);
            Assert.IsType<NoContentResult>(pricingStrategyItems);
        }

        [Fact]
        public void ShouldNot_PutPricingStrategyItem_ModelStateError()
        {
            PricingStrategyItem testPricingStrategyItem = new PricingStrategyItem { Id = 1 };

            Mock<IUnitOfWork> mock = new Mock<IUnitOfWork>();
            mock.Setup(f => f.PricingStrategyItems.Update(testPricingStrategyItem)).Returns(true);

            PricingStrategyItemsController controller = new PricingStrategyItemsController(mock.Object);
            controller.ModelState.AddModelError("TestError", "Error");
            var pricingStrategyItems = controller.PutPricingStrategyItem(1, testPricingStrategyItem);
            Assert.IsType<BadRequestObjectResult>(pricingStrategyItems);
        }

        [Fact]
        public void ShouldNot_PutPricingStrategyItem_IdMismatch()
        {
            PricingStrategyItem testPricingStrategyItem = new PricingStrategyItem { Id = 1 };

            Mock<IUnitOfWork> mock = new Mock<IUnitOfWork>();
            mock.Setup(f => f.PricingStrategyItems.Update(testPricingStrategyItem)).Returns(true);

            PricingStrategyItemsController controller = new PricingStrategyItemsController(mock.Object);
            var pricingStrategyItems = controller.PutPricingStrategyItem(2, testPricingStrategyItem);
            Assert.IsType<BadRequestResult>(pricingStrategyItems);
        }

        [Fact]
        public void Should_PostPricingStrategyItem()
        {
            PricingStrategyItem testPricingStrategyItem = new PricingStrategyItem { Id = 1, Name = "Test1", Description = "Test", Price = 5, PricingStrategyId = 1 };

            Mock<IUnitOfWork> mock = new Mock<IUnitOfWork>();
            mock.Setup(f => f.PricingStrategyItems.Create(testPricingStrategyItem)).Returns(true);

            PricingStrategyItemsController controller = new PricingStrategyItemsController(mock.Object);
            var pricingStrategyItems = controller.PostPricingStrategyItem(testPricingStrategyItem);
            Assert.IsType<CreatedAtActionResult>(pricingStrategyItems);

        }

        [Fact]
        public void ShouldNot_PostPricingStrategyItem_ModelStateError()
        {
            PricingStrategyItem testPricingStrategyItem = new PricingStrategyItem { Id = 1, Name = "Test1", Description = "Test", Price = 5, PricingStrategyId = 1 };

            Mock<IUnitOfWork> mock = new Mock<IUnitOfWork>();
            mock.Setup(f => f.PricingStrategyItems.Create(testPricingStrategyItem)).Returns(true);
            mock.Setup(f => f.PricingStrategyItems.GetById(1)).Returns(testPricingStrategyItem);

            PricingStrategyItemsController controller = new PricingStrategyItemsController(mock.Object);
            controller.ModelState.AddModelError("TestError", "Error");
            var pricingStrategyItems = controller.PostPricingStrategyItem(testPricingStrategyItem);
            Assert.IsType<BadRequestObjectResult>(pricingStrategyItems);
        }

        [Fact]
        public void Should_DeletePricingStrategyItem()
        {
            PricingStrategyItem testPricingStrategyItem = new PricingStrategyItem { Id = 1 };

            Mock<IUnitOfWork> mock = new Mock<IUnitOfWork>();
            mock.Setup(f => f.PricingStrategyItems.GetById(1)).Returns(testPricingStrategyItem);
            mock.Setup(f => f.PricingStrategyItems.Delete(testPricingStrategyItem)).Returns(true);

            PricingStrategyItemsController controller = new PricingStrategyItemsController(mock.Object);
            var result = controller.DeletePricingStrategyItem(1);
            Assert.IsType<OkObjectResult>(result);
        }

        [Fact]
        public void ShouldNot_DeletePricingStrategyItem_ModelStateError()
        {
            PricingStrategyItem testPricingStrategyItem = new PricingStrategyItem { Id = 1 };

            Mock<IUnitOfWork> mock = new Mock<IUnitOfWork>();
            mock.Setup(f => f.PricingStrategyItems.Delete(testPricingStrategyItem)).Returns(true);

            PricingStrategyItemsController controller = new PricingStrategyItemsController(mock.Object);
            controller.ModelState.AddModelError("TestError", "Error");
            var result = controller.DeletePricingStrategyItem(1);
            Assert.IsType<BadRequestObjectResult>(result);
        }

        [Fact]
        public void ShouldNot_DeletePricingStrategyItem_NotFound()
        {
            PricingStrategyItem testPricingStrategyItem = new PricingStrategyItem { Id = 10 };

            Mock<IUnitOfWork> mock = new Mock<IUnitOfWork>();
            mock.Setup(f => f.PricingStrategyItems.GetById(10)).Returns((PricingStrategyItem)null);

            PricingStrategyItemsController controller = new PricingStrategyItemsController(mock.Object);
            var result = controller.DeletePricingStrategyItem(10);
            Assert.IsType<NotFoundResult>(result);
        }

    }
}
