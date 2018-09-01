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
    public class PricingStrategiesController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;

        public PricingStrategiesController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        // GET: api/PricingStrategies
        [HttpGet]
        public IQueryable<PricingStrategy> GetPricingStrategies()
        {
            return _unitOfWork.PricingStrategies.Get();
        }

        // GET: api/PricingStrategies/5
        [HttpGet("{id}")]
        public IActionResult GetPricingStrategy([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var pricingStrategy = _unitOfWork.PricingStrategies.GetById(id);

            if (pricingStrategy == null)
            {
                return NotFound();
            }

            return Ok(pricingStrategy);
        }

        // PUT: api/PricingStrategies/5
        [Authorize(Roles = "admin")]
        [HttpPut("{id}")]
        public IActionResult PutPricingStrategy([FromRoute] int id, [FromBody] PricingStrategy pricingStrategy)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != pricingStrategy.Id)
            {
                return BadRequest();
            }

            try
            {
                _unitOfWork.PricingStrategies.Update(pricingStrategy);
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!PricingStrategiesExists(id))
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

        // POST: api/PricingStrategies
        [Authorize(Roles = "admin")]
        [HttpPost]
        public IActionResult PostPricingStrategy([FromBody] PricingStrategy pricingStrategy)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            _unitOfWork.PricingStrategies.Create(pricingStrategy);

            return CreatedAtAction("GetPricingStrategy", new { id = pricingStrategy.Id }, pricingStrategy);
        }

        // DELETE: api/PricingStrategies/5
        [Authorize(Roles = "admin")]
        [HttpDelete("{id}")]
        public IActionResult DeletePricingStrategy([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var pricingStrategy = _unitOfWork.PricingStrategies.GetById(id);
            if (pricingStrategy == null)
            {
                return NotFound();
            }

            _unitOfWork.PricingStrategies.Delete(pricingStrategy);

            return Ok(pricingStrategy);
        }

        [Authorize(Roles = "admin")]
        private bool PricingStrategiesExists(int id)
        {
            return _unitOfWork.PricingStrategies.Exists(new PricingStrategy { Id = id });
        }
    }
}