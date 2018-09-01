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

namespace AngularBooking.Controllers.Site
{
    [Route("api/[controller]")]
    [AutoValidateAntiforgeryToken]
    [ApiController]
    public class CustomersController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly UserManager<User> _userManager;

        public CustomersController(IUnitOfWork unitOfWork, UserManager<User> userManager)
        {
            _unitOfWork = unitOfWork;
            _userManager = userManager;
        }

        // GET: api/Customers
        [Authorize(Roles = "admin")]
        [HttpGet]
        public IQueryable<Customer> GetCustomers()
        {
            return _unitOfWork.Customers.Get();
        }

        // GET: api/Customers/5
        [Authorize(Roles = "admin")]
        [HttpGet("{id}")]
        public IActionResult GetCustomer([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var customer = _unitOfWork.Customers.GetById(id);

            if (customer == null)
            {
                return NotFound();
            }

            return Ok(customer);
        }

        // GET: api/Customer (authorized by current user)
        [Authorize]
        [HttpGet("/api/customer")]
        public IActionResult GetCustomer()
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            // get customer based upon sub claim (email address)
            Claim emailClaim = HttpContext.User.Claims.SingleOrDefault(f => f.Type == "sub");

            if(emailClaim == null)
            {
                return BadRequest();
            }

            User user = _userManager.FindByNameAsync(emailClaim.Value).Result;

            if (user == null)
            {
                return BadRequest();
            }

            var customer = _unitOfWork.Customers.Get().AsNoTracking().SingleOrDefault(f => f.UserId == user.Id);

            if (customer == null)
            {
                return NotFound();
            }

            return Ok(customer);
        }

        // PUT: api/Customers/5
        [Authorize(Roles = "admin")]
        [HttpPut("{id}")]
        public IActionResult PutCustomer([FromRoute] int id, [FromBody] Customer customer)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != customer.Id)
            {
                return BadRequest();
            }

            try
            {
                _unitOfWork.Customers.Update(customer);
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!CustomerExists(id))
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

        // PUT: api/Customer (authorized by current user)
        [Authorize]
        [HttpPut("/api/customer")]
        public IActionResult PutCurrentCustomer([FromBody] Customer customer)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            // confirm current user is linked to customer

            // get customer based upon sub claim (email address)
            Claim emailClaim = HttpContext.User.Claims.SingleOrDefault(f => f.Type == "sub");

            if (emailClaim == null)
            {
                return BadRequest(ModelState);
            }

            User user = _userManager.FindByNameAsync(emailClaim.Value).Result;

            bool valid = _unitOfWork.Customers.Get().Any(f => f.Id == customer.Id && f.UserId == user.Id);

            if(!valid)
            {
                return BadRequest();
            }

            try
            {
                // add userId to customer
                customer.UserId = user.Id;
                _unitOfWork.Customers.Update(customer);
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!CustomerExists(customer.Id))
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

        // POST: api/Customers
        [Authorize(Roles = "admin")]
        [HttpPost]
        public IActionResult PostCustomer([FromBody] Customer customer)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            _unitOfWork.Customers.Create(customer);

            return CreatedAtAction("GetCustomer", new { id = customer.Id }, customer);
        }

        // DELETE: api/Customers/5
        [Authorize(Roles = "admin")]
        [HttpDelete("{id}")]
        public IActionResult DeleteCustomer([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var customer = _unitOfWork.Customers.GetById(id);
            if (customer == null)
            {
                return NotFound();
            }

            // if deleted, remove user account if it exists
            User user = _userManager.FindByIdAsync(customer.UserId).Result;

            if (user != null)
            {
                // delete user
                var result = _userManager.DeleteAsync(user).Result;
                if (result.Succeeded)
                {
                    // delete customer
                    _unitOfWork.Customers.Delete(customer);
                    return Ok(customer);
                }
            }
            else
            {
                // no associated user, so ok to delete customer
                _unitOfWork.Customers.Delete(customer);
                return Ok(customer);
            }
            
            return BadRequest();
        }

        [Authorize(Roles = "admin")]
        private bool CustomerExists(int id)
        {
            return _unitOfWork.Customers.Exists(new Customer { Id = id });
        }
    }
}