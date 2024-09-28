using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Quik_BookingApp.Repos;
using Quik_BookingApp.Service;

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

        //GET: api/Business
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

        // GET: api/Business/{id}
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
