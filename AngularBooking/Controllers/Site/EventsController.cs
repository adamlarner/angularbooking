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
    public class EventsController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;

        public EventsController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        // GET: api/Events
        [HttpGet]
        [EnableQuery()]
        public IQueryable<Event> GetEvents()
        {
            return _unitOfWork.Events.Get();
        }

        // GET: api/Events/5
        [HttpGet("{id}")]
        public IActionResult GetEvent([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var @event = _unitOfWork.Events.GetById(id);

            if (@event == null)
            {
                return NotFound();
            }

            return Ok(@event);
        }

        // PUT: api/Events/5
        [Authorize(Roles = "admin")]
        [HttpPut("{id}")]
        public IActionResult PutEvent([FromRoute] int id, [FromBody] Event @event)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != @event.Id)
            {
                return BadRequest();
            }

            try
            {
                _unitOfWork.Events.Update(@event);
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!EventExists(id))
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

        // POST: api/Events
        [Authorize(Roles = "admin")]
        [HttpPost]
        public IActionResult PostEvent([FromBody] Event @event)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            _unitOfWork.Events.Create(@event);

            return CreatedAtAction("GetEvent", new { id = @event.Id }, @event);
        }

        // DELETE: api/Events/5
        [Authorize(Roles = "admin")]
        [HttpDelete("{id}")]
        public IActionResult DeleteEvent([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var @event = _unitOfWork.Events.GetById(id);
            if (@event == null)
            {
                return NotFound();
            }

            _unitOfWork.Events.Delete(@event);

            return Ok(@event);
        }

        [Authorize(Roles = "admin")]
        private bool EventExists(int id)
        {
            return _unitOfWork.Events.Exists(new Event { Id = id });
        }
    }
}