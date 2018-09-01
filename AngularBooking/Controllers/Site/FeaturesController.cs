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
    public class FeaturesController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;

        public FeaturesController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        // GET: api/Features
        [HttpGet]
        [EnableQuery()]
        public IQueryable<Feature> GetFeatures()
        {
            return _unitOfWork.Features.Get();
        }

        // GET: api/Features/5
        [HttpGet("{id}")]
        public IActionResult GetFeature([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var feature = _unitOfWork.Features.GetById(id);

            if (feature == null)
            {
                return NotFound();
            }

            return Ok(feature);
        }

        // PUT: api/Features/5
        [Authorize(Roles = "admin")]
        [HttpPut("{id}")]
        public IActionResult PutFeature([FromRoute] int id, [FromBody] Feature feature)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != feature.Id)
            {
                return BadRequest();
            }

            try
            {
                _unitOfWork.Features.Update(feature);
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!FeatureExists(id))
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

        // POST: api/Features
        [Authorize(Roles = "admin")]
        [HttpPost]
        public IActionResult PostFeature([FromBody] Feature feature)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            _unitOfWork.Features.Create(feature);

            return CreatedAtAction("GetFeature", new { id = feature.Id }, feature);
        }

        // DELETE: api/Features/5
        [Authorize(Roles = "admin")]
        [HttpDelete("{id}")]
        public IActionResult DeleteFeature([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var feature = _unitOfWork.Features.GetById(id);
            if (feature == null)
            {
                return NotFound();
            }

            _unitOfWork.Features.Delete(feature);

            return Ok(feature);
        }

        [Authorize(Roles = "admin")]
        private bool FeatureExists(int id)
        {
            return _unitOfWork.Features.Exists(new Feature { Id = id });
        }
    }
}