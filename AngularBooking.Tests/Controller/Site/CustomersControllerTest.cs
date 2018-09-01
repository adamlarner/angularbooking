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
    public class CustomersControllerTest
    {
        [Fact]
        public void Should_GetCustomers()
        {
            // mock UoW and repository data
            Mock<IUnitOfWork> mock = new Mock<IUnitOfWork>();
            mock.Setup(f => f.Customers.Get()).Returns(new List<Customer>
            {
                new Customer
                {
                    Id = 1,
                    UserId = Guid.NewGuid().ToString(),
                    Address1 = "Addr1",
                    Address2 = "Addr2",
                    Address3 = "Addr3",
                    Address4 = "Addr4",
                    Address5 = "Addr5",
                    ContactEmail = "test@test.com",
                    ContactPhone = "01234567898",
                    FirstName = "First",
                    LastName = "Last"
                },
                new Customer
                {
                    Id = 2,
                    UserId = Guid.NewGuid().ToString(),
                    Address1 = "Addr1",
                    Address2 = "Addr2",
                    Address3 = "Addr3",
                    Address4 = "Addr4",
                    Address5 = "Addr5",
                    ContactEmail = "test@test.com",
                    ContactPhone = "01234567898",
                    FirstName = "First",
                    LastName = "Last"
                },
                new Customer
                {
                    Id = 3,
                    UserId = Guid.NewGuid().ToString(),
                    Address1 = "Addr1",
                    Address2 = "Addr2",
                    Address3 = "Addr3",
                    Address4 = "Addr4",
                    Address5 = "Addr5",
                    ContactEmail = "test@test.com",
                    ContactPhone = "01234567898",
                    FirstName = "First",
                    LastName = "Last"
                }
            }
            .AsQueryable());

            CustomersController controller = new CustomersController(mock.Object, null);
            var customers = controller.GetCustomers();
            Assert.True(customers.Count() == 3);
        }

        [Fact]
        public void Should_GetCustomer()
        {
            Customer testCustomer = new Customer
            {
                Id = 1,
                UserId = Guid.NewGuid().ToString(),
                Address1 = "Addr1",
                Address2 = "Addr2",
                Address3 = "Addr3",
                Address4 = "Addr4",
                Address5 = "Addr5",
                ContactEmail = "test@test.com",
                ContactPhone = "01234567898",
                FirstName = "First",
                LastName = "Last"
            };

            Mock<IUnitOfWork> mock = new Mock<IUnitOfWork>();
            mock.Setup(f => f.Customers.GetById(1)).Returns(testCustomer);

            CustomersController controller = new CustomersController(mock.Object, null);
            var customer = controller.GetCustomer(1);
            Assert.IsType<OkObjectResult>(customer);
        }

        [Fact]
        public void Should_GetCustomer_ForAuthorizedUser()
        {
            Customer testCustomer = new Customer
            {
                Id = 1,
                UserId = "test_user",
                Address1 = "Addr1",
                Address2 = "Addr2",
                Address3 = "Addr3",
                Address4 = "Addr4",
                Address5 = "Addr5",
                ContactEmail = "test@test.com",
                ContactPhone = "01234567898",
                FirstName = "First",
                LastName = "Last",
            };

            Mock<IUnitOfWork> mockUOW = new Mock<IUnitOfWork>();
            mockUOW.Setup(f => f.Customers.Get()).Returns(new[] { testCustomer }.AsQueryable());

            var userStore = new Mock<IUserStore<User>>();
            Mock<UserManager<User>> mockUserManager = new Mock<UserManager<User>>(userStore.Object, null, null, null, null, null, null, null, null);
            mockUserManager.Setup(f => f.GetUserId(It.IsAny<ClaimsPrincipal>())).Returns("test_user");
            mockUserManager.Setup(f => f.FindByNameAsync(It.IsAny<string>())).Returns(Task.FromResult(new User { Id = "test_user" }));

            CustomersController controller = new CustomersController(mockUOW.Object, mockUserManager.Object);
            var userClaimsIdentity = new ClaimsPrincipal();
            userClaimsIdentity.AddIdentity(new ClaimsIdentity(new[] { new Claim("sub", "test@test.com") }));
            controller.ControllerContext.HttpContext = new DefaultHttpContext() { User = userClaimsIdentity };
            var customer = controller.GetCustomer();
            Assert.IsType<OkObjectResult>(customer);
        }

        [Fact]
        public void ShouldNot_GetCustomer_UnauthorizedUser()
        {
            Customer testCustomer = new Customer
            {
                Id = 1,
                UserId = "test_user",
                Address1 = "Addr1",
                Address2 = "Addr2",
                Address3 = "Addr3",
                Address4 = "Addr4",
                Address5 = "Addr5",
                ContactEmail = "test@test.com",
                ContactPhone = "01234567898",
                FirstName = "First",
                LastName = "Last",
            };

            Mock<IUnitOfWork> mockUOW = new Mock<IUnitOfWork>();
            mockUOW.Setup(f => f.Customers.Get()).Returns(new[] { testCustomer }.AsQueryable());

            var userStore = new Mock<IUserStore<User>>();
            Mock<UserManager<User>> mockUserManager = new Mock<UserManager<User>>(userStore.Object, null, null, null, null, null, null, null, null);
            mockUserManager.Setup(f => f.GetUserId(It.IsAny<ClaimsPrincipal>())).Returns((string)null);
            mockUserManager.Setup(f => f.FindByNameAsync(It.IsAny<string>())).Returns(Task.FromResult(new User { Id = "test_user" }));

            CustomersController controller = new CustomersController(mockUOW.Object, mockUserManager.Object);
            controller.ControllerContext.HttpContext = new DefaultHttpContext() { User = new ClaimsPrincipal() };
            var customer = controller.GetCustomer();
            Assert.IsType<BadRequestResult>(customer);
        }

        [Fact]
        public void Should_PutCustomer()
        {
            Customer testCustomer = new Customer
            {
                Id = 1,
                UserId = Guid.NewGuid().ToString(),
                Address1 = "Addr1",
                Address2 = "Addr2",
                Address3 = "Addr3",
                Address4 = "Addr4",
                Address5 = "Addr5",
                ContactEmail = "test@test.com",
                ContactPhone = "01234567898",
                FirstName = "First",
                LastName = "Last"
            };

            Mock<IUnitOfWork> mock = new Mock<IUnitOfWork>();
            mock.Setup(f => f.Customers.Update(testCustomer)).Returns(true);

            CustomersController controller = new CustomersController(mock.Object, null);
            var customers = controller.PutCustomer(1, testCustomer);
            Assert.IsType<NoContentResult>(customers);
        }

        [Fact]
        public void ShouldNot_PutCustomer_ModelStateError()
        {
            Customer testCustomer = new Customer
            {
                Id = 1,
                UserId = Guid.NewGuid().ToString(),
                Address1 = "Addr1",
                Address2 = "Addr2",
                Address3 = "Addr3",
                Address4 = "Addr4",
                Address5 = "Addr5",
                ContactEmail = "test@test.com",
                ContactPhone = "01234567898",
                FirstName = "First",
                LastName = "Last"
            };

            Mock<IUnitOfWork> mock = new Mock<IUnitOfWork>();
            mock.Setup(f => f.Customers.Update(testCustomer)).Returns(true);

            CustomersController controller = new CustomersController(mock.Object, null);
            controller.ModelState.AddModelError("TestError", "Error");
            var customers = controller.PutCustomer(1, testCustomer);
            Assert.IsType<BadRequestObjectResult>(customers);
        }

        [Fact]
        public void ShouldNot_PutCustomer_IdMismatch()
        {
            Customer testCustomer = new Customer
            {
                Id = 1,
                UserId = Guid.NewGuid().ToString(),
                Address1 = "Addr1",
                Address2 = "Addr2",
                Address3 = "Addr3",
                Address4 = "Addr4",
                Address5 = "Addr5",
                ContactEmail = "test@test.com",
                ContactPhone = "01234567898",
                FirstName = "First",
                LastName = "Last"
            };

            Mock<IUnitOfWork> mock = new Mock<IUnitOfWork>();
            mock.Setup(f => f.Customers.Update(testCustomer)).Returns(true);

            CustomersController controller = new CustomersController(mock.Object, null);
            var customers = controller.PutCustomer(2, testCustomer);
            Assert.IsType<BadRequestResult>(customers);
        }

        [Fact]
        public void Should_PostCustomer()
        {
            Customer testCustomer = new Customer
            {
                Id = 1,
                UserId = Guid.NewGuid().ToString(),
                Address1 = "Addr1",
                Address2 = "Addr2",
                Address3 = "Addr3",
                Address4 = "Addr4",
                Address5 = "Addr5",
                ContactEmail = "test@test.com",
                ContactPhone = "01234567898",
                FirstName = "First",
                LastName = "Last"
            };

            Mock<IUnitOfWork> mock = new Mock<IUnitOfWork>();
            mock.Setup(f => f.Customers.Create(testCustomer)).Returns(true);

            CustomersController controller = new CustomersController(mock.Object, null);
            var customers = controller.PostCustomer(testCustomer);
            Assert.IsType<CreatedAtActionResult>(customers);

        }

        [Fact]
        public void ShouldNot_PostCustomer_ModelStateError()
        {
            Customer testCustomer = new Customer
            {
                Id = 1,
                UserId = Guid.NewGuid().ToString(),
                Address1 = "Addr1",
                Address2 = "Addr2",
                Address3 = "Addr3",
                Address4 = "Addr4",
                Address5 = "Addr5",
                ContactEmail = "test@test.com",
                ContactPhone = "01234567898",
                FirstName = "First",
                LastName = "Last"
            };

            Mock<IUnitOfWork> mock = new Mock<IUnitOfWork>();
            mock.Setup(f => f.Customers.Create(testCustomer)).Returns(true);
            mock.Setup(f => f.Customers.GetById(1)).Returns(testCustomer);

            CustomersController controller = new CustomersController(mock.Object, null);
            controller.ModelState.AddModelError("TestError", "Error");
            var customers = controller.PostCustomer(testCustomer);
            Assert.IsType<BadRequestObjectResult>(customers);
        }

        [Fact]
        public void Should_DeleteCustomer()
        {
            Customer testCustomer = new Customer { Id = 1 };

            Mock<IUnitOfWork> mock = new Mock<IUnitOfWork>();
            mock.Setup(f => f.Customers.GetById(1)).Returns(testCustomer);
            mock.Setup(f => f.Customers.Delete(testCustomer)).Returns(true);

            var userStore = new Mock<IUserStore<User>>();
            Mock<UserManager<User>> mockUserManager = new Mock<UserManager<User>>(userStore.Object, null, null, null, null, null, null, null, null);
            mockUserManager.Setup(f => f.FindByIdAsync(It.IsAny<string>())).Returns(Task.FromResult(new User { Id = "test" }));
            mockUserManager.Setup(f => f.DeleteAsync(It.IsAny<User>())).Returns(Task.FromResult(IdentityResult.Success));

            CustomersController controller = new CustomersController(mock.Object, mockUserManager.Object);
            var result = controller.DeleteCustomer(1);
            Assert.IsType<OkObjectResult>(result);
        }

        [Fact]
        public void ShouldNot_DeleteCustomer_ModelStateError()
        {
            Customer testCustomer = new Customer { Id = 1 };

            Mock<IUnitOfWork> mock = new Mock<IUnitOfWork>();
            mock.Setup(f => f.Customers.Delete(testCustomer)).Returns(true);

            CustomersController controller = new CustomersController(mock.Object, null);
            controller.ModelState.AddModelError("TestError", "Error");
            var result = controller.DeleteCustomer(1);
            Assert.IsType<BadRequestObjectResult>(result);
        }

        [Fact]
        public void ShouldNot_DeleteCustomer_NotFound()
        {
            Customer testCustomer = new Customer { Id = 10 };

            Mock<IUnitOfWork> mock = new Mock<IUnitOfWork>();
            mock.Setup(f => f.Customers.GetById(10)).Returns((Customer)null);

            CustomersController controller = new CustomersController(mock.Object, null);
            var result = controller.DeleteCustomer(10);
            Assert.IsType<NotFoundResult>(result);
        }

    }
}
