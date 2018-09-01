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
    public class PricingStrategyItemsController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;

        public PricingStrategyItemsController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        // GET: api/PricingStrategyItems
        [HttpGet]
        public IQueryable<PricingStrategyItem> GetPricingStrategyItems()
        {
            return _unitOfWork.PricingStrategyItems.Get();
        }

        // GET: api/PricingStrategyItems/5
        [HttpGet("{id}")]
        public IActionResult GetPricingStrategyItem([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var pricingStrategyItem = _unitOfWork.PricingStrategyItems.GetById(id);

            if (pricingStrategyItem == null)
            {
                return NotFound();
            }

            return Ok(pricingStrategyItem);
        }

        // PUT: api/PricingStrategyItems/5
        [Authorize(Roles = "admin")]
        [HttpPut("{id}")]
        public IActionResult PutPricingStrategyItem([FromRoute] int id, [FromBody] PricingStrategyItem pricingStrategyItem)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != pricingStrategyItem.Id)
            {
                return BadRequest();
            }

            try
            {
                _unitOfWork.PricingStrategyItems.Update(pricingStrategyItem);
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!PricingStrategyItemExists(id))
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

        // POST: api/PricingStrategyItems
        [Authorize(Roles = "admin")]
        [HttpPost]
        public IActionResult PostPricingStrategyItem([FromBody] PricingStrategyItem pricingStrategyItem)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            _unitOfWork.PricingStrategyItems.Create(pricingStrategyItem);

            return CreatedAtAction("GetPricingStrategyItem", new { id = pricingStrategyItem.Id }, pricingStrategyItem);
        }

        // DELETE: api/PricingStrategyItems/5
        [Authorize(Roles = "admin")]
        [HttpDelete("{id}")]
        public IActionResult DeletePricingStrategyItem([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var pricingStrategyItem = _unitOfWork.PricingStrategyItems.GetById(id);
            if (pricingStrategyItem == null)
            {
                return NotFound();
            }

            _unitOfWork.PricingStrategyItems.Delete(pricingStrategyItem);

            return Ok(pricingStrategyItem);
        }

        [Authorize(Roles = "admin")]
        private bool PricingStrategyItemExists(int id)
        {
            return _unitOfWork.PricingStrategyItems.Exists(new PricingStrategyItem { Id = id });
        }
    }
}