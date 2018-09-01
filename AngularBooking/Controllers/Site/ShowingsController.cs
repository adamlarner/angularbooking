using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using AngularBooking.Data;
using AngularBooking.Models;
using Microsoft.AspNet.OData;
using Microsoft.AspNetCore.Authorization;

namespace AngularBooking.Controllers.Site
{
    [Route("api/[controller]")]
    [AutoValidateAntiforgeryToken]
    [ApiController]
    public class ShowingsController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;

        public ShowingsController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        // GET: api/Showings
        [HttpGet]
        [EnableQuery()]
        public IQueryable<Showing> GetShowings()
        {
            return _unitOfWork.Showings.Get();
        }

        // GET: api/Showings/5
        [HttpGet("{id}")]
        public IActionResult GetShowing([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var showing = _unitOfWork.Showings.GetById(id);

            if (showing == null)
            {
                return NotFound();
            }

            return Ok(showing);
        }

        // PUT: api/Showings/5
        [Authorize(Roles = "admin")]
        [HttpPut("{id}")]
        public IActionResult PutShowing([FromRoute] int id, [FromBody] Showing showing)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != showing.Id)
            {
                return BadRequest();
            }

            try
            {
                _unitOfWork.Showings.Update(showing);
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ShowingExists(id))
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

        // POST: api/Showings
        [Authorize(Roles = "admin")]
        [HttpPost]
        public IActionResult PostShowing([FromBody] Showing showing)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            _unitOfWork.Showings.Create(showing);

            return CreatedAtAction("GetShowing", new { id = showing.Id }, showing);
        }

        // DELETE: api/Showings/5
        [Authorize(Roles = "admin")]
        [HttpDelete("{id}")]
        public IActionResult DeleteShowing([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var showing = _unitOfWork.Showings.GetById(id);
            if (showing == null)
            {
                return NotFound();
            }

            _unitOfWork.Showings.Delete(showing);

            return Ok(showing);
        }

        [HttpGet("allocations/{id}")]
        public IActionResult GetAllocatedSeating([FromRoute] int id)
        {
            // confirm showing exists
            var showing = _unitOfWork.Showings.Get().Include(i => i.Room).SingleOrDefault(f => f.Id == id);

            if (showing == null)
                return NotFound();

            // get all booking items for 
            var bookingItems = _unitOfWork.Bookings.Get().Where(f => f.ShowingId == id).SelectMany(s => s.BookingItems);

            List<int> allocatedSeating = bookingItems.Select(f => f.Location).ToList();

            return new JsonResult(allocatedSeating);
        }

        [Authorize(Roles = "admin")]
        private bool ShowingExists(int id)
        {
            return _unitOfWork.Showings.Exists(new Showing { Id = id });
        }
    }
}