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
    public class VenuesController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;

        public VenuesController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        // GET: api/Venues
        [HttpGet]
        [EnableQuery(MaxExpansionDepth = 0, MaxAnyAllExpressionDepth = 2)]
        public IQueryable<Venue> GetVenues()
        {
            return _unitOfWork.Venues.Get();
        }

        // GET: api/Venues/5
        [HttpGet("{id}")]
        public IActionResult GetVenue([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var venue = _unitOfWork.Venues.GetById(id);

            if (venue == null)
            {
                return NotFound();
            }

            return Ok(venue);
        }

        // PUT: api/Venues/5
        [Authorize(Roles = "admin")]
        [HttpPut("{id}")]
        public IActionResult PutVenue([FromRoute] int id, [FromBody] Venue venue)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != venue.Id)
            {
                return BadRequest();
            }

            try
            {
                _unitOfWork.Venues.Update(venue);
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!VenueExists(id))
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

        // POST: api/Venues
        [Authorize(Roles = "admin")]
        [HttpPost]
        public IActionResult PostVenue([FromBody] Venue venue)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            _unitOfWork.Venues.Create(venue);

            return CreatedAtAction("GetVenue", new { id = venue.Id }, venue);
        }

        // DELETE: api/Venues/5
        [Authorize(Roles = "admin")]
        [HttpDelete("{id}")]
        public IActionResult DeleteVenue([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var venue = _unitOfWork.Venues.GetById(id);
            if (venue == null)
            {
                return NotFound();
            }

            _unitOfWork.Venues.Delete(venue);

            return Ok(venue);
        }

        [Authorize(Roles = "admin")]
        private bool VenueExists(int id)
        {
            return _unitOfWork.Venues.Exists(new Venue { Id = id });
        }
    }
}