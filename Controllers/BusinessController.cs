using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Quik_BookingApp.DAO;
using Quik_BookingApp.DAO.Models;
using Quik_BookingApp.Repos.Interface;
using Swashbuckle.AspNetCore.Annotations;

namespace Quik_BookingApp.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class BusinessController : ControllerBase
    {
        private readonly IBusinessService _businessService;

        public BusinessController(IBusinessService businessService)
        {
            _businessService = businessService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllBusinesses()
        {
            var businesses = await _businessService.GetAllBusinessesAsync();
            return Ok(businesses);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetBusinessById(string id)
        {
            var business = await _businessService.GetBusinessByIdAsync(id);
            if (business == null) return NotFound();
            return Ok(business);
        }

        [HttpPost]
        public async Task<IActionResult> CreateBusiness([FromBody] Business business)
        {
            var createdBusiness = await _businessService.CreateBusinessAsync(business);
            return CreatedAtAction(nameof(GetBusinessById), new { id = createdBusiness.BusinessId }, createdBusiness);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateBusiness(string id, [FromBody] Business business)
        {
            if (id != business.BusinessId) return BadRequest();

            var updatedBusiness = await _businessService.UpdateBusinessAsync(business);
            if (updatedBusiness == null) return NotFound();

            return Ok(updatedBusiness);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteBusiness(string id)
        {
            var success = await _businessService.DeleteBusinessAsync(id);
            if (!success) return NotFound();

            return NoContent();
        }
    }


}

