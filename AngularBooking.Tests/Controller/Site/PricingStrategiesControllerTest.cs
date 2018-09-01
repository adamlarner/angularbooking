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
    public class PricingStrategiesControllerTest
    {
        [Fact]
        public void Should_GetPricingStrategies()
        {
            // mock UoW and repository data
            Mock<IUnitOfWork> mock = new Mock<IUnitOfWork>();
            mock.Setup(f => f.PricingStrategies.Get()).Returns(new List<PricingStrategy>
            {
                new PricingStrategy { Id = 1, Name = "Test1", Description = "Test", PricingStrategyItems = new List<PricingStrategyItem>() },
                new PricingStrategy { Id = 2, Name = "Test2", Description = "Test", PricingStrategyItems = new List<PricingStrategyItem>() },
                new PricingStrategy { Id = 3, Name = "Test3", Description = "Test", PricingStrategyItems = new List<PricingStrategyItem>() },
                new PricingStrategy { Id = 4, Name = "Test4", Description = "Test", PricingStrategyItems = new List<PricingStrategyItem>() },
                new PricingStrategy { Id = 5, Name = "Test5", Description = "Test", PricingStrategyItems = new List<PricingStrategyItem>() },
            }
            .AsQueryable());

            PricingStrategiesController controller = new PricingStrategiesController(mock.Object);
            var pricingStrategies = controller.GetPricingStrategies();
            Assert.True(pricingStrategies.Count() == 5);
        }

        [Fact]
        public void Should_GetPricingStrategy()
        {
            PricingStrategy testPricingStrategy = new PricingStrategy { Id = 1, Name = "Test1", Description = "Test", PricingStrategyItems = new List<PricingStrategyItem>() };

            Mock<IUnitOfWork> mock = new Mock<IUnitOfWork>();
            mock.Setup(f => f.PricingStrategies.GetById(1)).Returns(testPricingStrategy);

            PricingStrategiesController controller = new PricingStrategiesController(mock.Object);
            var pricingStrategy = controller.GetPricingStrategy(1);
            Assert.IsType<OkObjectResult>(pricingStrategy);
        }

        [Fact]
        public void Should_PutPricingStrategy()
        {
            PricingStrategy testPricingStrategy = new PricingStrategy { Id = 1 };

            Mock<IUnitOfWork> mock = new Mock<IUnitOfWork>();
            mock.Setup(f => f.PricingStrategies.Update(testPricingStrategy)).Returns(true);

            PricingStrategiesController controller = new PricingStrategiesController(mock.Object);
            var pricingStrategies = controller.PutPricingStrategy(1, testPricingStrategy);
            Assert.IsType<NoContentResult>(pricingStrategies);
        }

        [Fact]
        public void ShouldNot_PutPricingStrategy_ModelStateError()
        {
            PricingStrategy testPricingStrategy = new PricingStrategy { Id = 1 };

            Mock<IUnitOfWork> mock = new Mock<IUnitOfWork>();
            mock.Setup(f => f.PricingStrategies.Update(testPricingStrategy)).Returns(true);

            PricingStrategiesController controller = new PricingStrategiesController(mock.Object);
            controller.ModelState.AddModelError("TestError", "Error");
            var pricingStrategy = controller.PutPricingStrategy(1, testPricingStrategy);
            Assert.IsType<BadRequestObjectResult>(pricingStrategy);
        }

        [Fact]
        public void ShouldNot_PutPricingStrategy_IdMismatch()
        {
            PricingStrategy testPricingStrategy = new PricingStrategy { Id = 1 };

            Mock<IUnitOfWork> mock = new Mock<IUnitOfWork>();
            mock.Setup(f => f.PricingStrategies.Update(testPricingStrategy)).Returns(true);

            PricingStrategiesController controller = new PricingStrategiesController(mock.Object);
            var pricingStrategy = controller.PutPricingStrategy(2, testPricingStrategy);
            Assert.IsType<BadRequestResult>(pricingStrategy);
        }

        [Fact]
        public void Should_PostPricingStrategy()
        {
            PricingStrategy testPricingStrategy = new PricingStrategy { Id = 1, Name = "Test1", Description = "Test", PricingStrategyItems = new List<PricingStrategyItem>() };

            Mock<IUnitOfWork> mock = new Mock<IUnitOfWork>();
            mock.Setup(f => f.PricingStrategies.Create(testPricingStrategy)).Returns(true);

            PricingStrategiesController controller = new PricingStrategiesController(mock.Object);
            var pricingStrategy = controller.PostPricingStrategy(testPricingStrategy);
            Assert.IsType<CreatedAtActionResult>(pricingStrategy);
        }

        [Fact]
        public void ShouldNot_PostPricingStrategy_ModelStateError()
        {
            PricingStrategy testPricingStrategy = new PricingStrategy { Id = 1, Name = "Test1", Description = "Test", PricingStrategyItems = new List<PricingStrategyItem>() };

            Mock<IUnitOfWork> mock = new Mock<IUnitOfWork>();
            mock.Setup(f => f.PricingStrategies.Create(testPricingStrategy)).Returns(true);
            mock.Setup(f => f.PricingStrategies.GetById(1)).Returns(testPricingStrategy);

            PricingStrategiesController controller = new PricingStrategiesController(mock.Object);
            controller.ModelState.AddModelError("TestError", "Error");
            var pricingStrategy = controller.PostPricingStrategy(testPricingStrategy);
            Assert.IsType<BadRequestObjectResult>(pricingStrategy);
        }

        [Fact]
        public void Should_DeletePricingStrategy()
        {
            PricingStrategy testPricingStrategy = new PricingStrategy { Id = 1 };

            Mock<IUnitOfWork> mock = new Mock<IUnitOfWork>();
            mock.Setup(f => f.PricingStrategies.GetById(1)).Returns(testPricingStrategy);
            mock.Setup(f => f.PricingStrategies.Delete(testPricingStrategy)).Returns(true);

            PricingStrategiesController controller = new PricingStrategiesController(mock.Object);
            var result = controller.DeletePricingStrategy(1);
            Assert.IsType<OkObjectResult>(result);
        }

        [Fact]
        public void ShouldNot_DeletePricingStrategy_ModelStateError()
        {
            PricingStrategy testPricingStrategy = new PricingStrategy { Id = 1 };

            Mock<IUnitOfWork> mock = new Mock<IUnitOfWork>();
            mock.Setup(f => f.PricingStrategies.Delete(testPricingStrategy)).Returns(true);

            PricingStrategiesController controller = new PricingStrategiesController(mock.Object);
            controller.ModelState.AddModelError("TestError", "Error");
            var result = controller.DeletePricingStrategy(1);
            Assert.IsType<BadRequestObjectResult>(result);
        }

        [Fact]
        public void ShouldNot_DeletePricingStrategy_NotFound()
        {
            PricingStrategy testPricingStrategy = new PricingStrategy { Id = 10 };

            Mock<IUnitOfWork> mock = new Mock<IUnitOfWork>();
            mock.Setup(f => f.PricingStrategies.GetById(10)).Returns((PricingStrategy)null);

            PricingStrategiesController controller = new PricingStrategiesController(mock.Object);
            var result = controller.DeletePricingStrategy(10);
            Assert.IsType<NotFoundResult>(result);
        }

    }
}
