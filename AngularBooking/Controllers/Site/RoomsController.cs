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
    public class RoomsController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;

        public RoomsController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        // GET: api/Rooms
        [HttpGet]
        [EnableQuery()]
        public IQueryable<Room> GetRooms()
        {
            return _unitOfWork.Rooms.Get();
        }

        // GET: api/Rooms/5
        [HttpGet("{id}")]
        public IActionResult GetRoom([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var room = _unitOfWork.Rooms.GetById(id);

            if (room == null)
            {
                return NotFound();
            }

            return Ok(room);
        }

        // PUT: api/Rooms/5
        [Authorize(Roles = "admin")]
        [HttpPut("{id}")]
        public IActionResult PutRoom([FromRoute] int id, [FromBody] Room room)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != room.Id)
            {
                return BadRequest();
            }

            try
            {
                _unitOfWork.Rooms.Update(room);
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!RoomExists(id))
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

        // POST: api/Rooms
        [Authorize(Roles = "admin")]
        [HttpPost]
        public IActionResult PostRoom([FromBody] Room room)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            _unitOfWork.Rooms.Create(room);

            return CreatedAtAction("GetRoom", new { id = room.Id }, room);
        }

        // DELETE: api/Rooms/5
        [Authorize(Roles = "admin")]
        [HttpDelete("{id}")]
        public IActionResult DeleteRoom([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var room = _unitOfWork.Rooms.GetById(id);
            if (room == null)
            {
                return NotFound();
            }

            _unitOfWork.Rooms.Delete(room);

            return Ok(room);
        }

        [Authorize(Roles = "admin")]
        private bool RoomExists(int id)
        {
            return _unitOfWork.Rooms.Exists(new Room { Id = id });
        }
    }
}