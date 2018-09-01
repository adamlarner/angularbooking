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

namespace AngularBooking.Controllers.Site
{
    [Route("api/[controller]")]
    [AutoValidateAntiforgeryToken]
    [ApiController]
    public class BookingItemsController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;

        public BookingItemsController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        // GET: api/BookingItems
        [Authorize(Roles = "admin")]
        [HttpGet]
        public IQueryable<BookingItem> GetBookingItems()
        {
            return _unitOfWork.BookingItems.Get();
        }

        // GET: api/BookingItems/5
        [Authorize(Roles = "admin")]
        [HttpGet("{id}")]
        public IActionResult GetBookingItem([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var bookingItem = _unitOfWork.BookingItems.GetById(id);

            if (bookingItem == null)
            {
                return NotFound();
            }

            return Ok(bookingItem);
        }

        // PUT: api/BookingItems/5
        [Authorize(Roles = "admin")]
        [HttpPut("{id}")]
        public IActionResult PutBookingItem([FromRoute] int id, [FromBody] BookingItem bookingItem)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != bookingItem.Id)
            {
                return BadRequest();
            }

            try
            {
                _unitOfWork.BookingItems.Update(bookingItem);
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!BookingItemExists(id))
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

        // POST: api/BookingItems
        [Authorize(Roles = "admin")]
        [HttpPost]
        public IActionResult PostBookingItem([FromBody] BookingItem bookingItem)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            _unitOfWork.BookingItems.Create(bookingItem);

            return CreatedAtAction("GetBookingItem", new { id = bookingItem.Id }, bookingItem);
        }

        // DELETE: api/BookingItems/5
        [Authorize(Roles = "admin")]
        [HttpDelete("{id}")]
        public IActionResult DeleteBookingItem([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var bookingItem = _unitOfWork.BookingItems.GetById(id);
            if (bookingItem == null)
            {
                return NotFound();
            }

            _unitOfWork.BookingItems.Delete(bookingItem);

            return Ok(bookingItem);
        }

        [Authorize(Roles = "admin")]
        private bool BookingItemExists(int id)
        {
            return _unitOfWork.BookingItems.Exists(new BookingItem { Id = id });
        }
    }
}