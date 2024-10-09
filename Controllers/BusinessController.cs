using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Quik_BookingApp.DAO;
using Quik_BookingApp.Repos.Interface;
using Swashbuckle.AspNetCore.Annotations;

namespace Quik_BookingApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BusinessController : ControllerBase
    {
        private readonly IBusinessService _service;
        private readonly QuikDbContext _dbContext;

        public BusinessController(IBusinessService service, QuikDbContext _dbContext)
        {
            this._service = service;
            this._dbContext = _dbContext;
        }

        [SwaggerOperation(
            Summary = "Retrieve all businesses",
            Description = "Gets a list of all registered businesses. If no businesses are found, a 404 Not Found response is returned."
        )]
        [HttpGet("GetAllBusiness")]
        public async Task<IActionResult> GetAllBusiness()
        {
            var data = await _service.GetAllBusiness();
            if (data == null || data.Count == 0)
            {
                return NotFound();
            }
            return Ok(data);
        }

        [SwaggerOperation(
             Summary = "Retrieve all working spaces of a business",
             Description = "Gets a list of all working spaces. If no working spaces are found, a 404 Not Found response is returned."
        )]
        [HttpGet("GetWSsOfBusiness")]
        public async Task<IActionResult> GetListWSOfBusiness(string businessId)
        {
            try
            {
                var data = await _service.GetListWSOfBusiness(businessId);

                // If no data is found, return a NotFound response
                if (data == null || !data.Any())
                {
                    return NotFound("No working spaces found for this business.");
                }

                return Ok(data);
            }
            catch (Exception ex)
            {
                // Log the exception (optional)
                return StatusCode(500, "An error occurred while processing your request: " + ex.Message);
            }
        }


        [SwaggerOperation(
            Summary = "Retrieve a business by ID",
            Description = "Gets a business's details by providing the business ID. If the business is not found, a 404 Not Found response is returned."
        )]
        [HttpGet("{id}")]
        public async Task<IActionResult> GetBusinessById(string id)
        {
            var booking = await _service.GetBusinessById(id);
            if (booking == null)
            {
                return NotFound("Business not found.");
            }

            return Ok(booking);
        }

    }
}
