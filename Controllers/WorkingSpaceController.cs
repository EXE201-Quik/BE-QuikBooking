using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Quik_BookingApp.Models;
using Quik_BookingApp.Service;

namespace Quik_BookingApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class WorkingSpaceController : ControllerBase
    {
        private readonly IWorkingSpaceService workingSpaceService;

        public WorkingSpaceController(IWorkingSpaceService workingSpaceService)
        {
            this.workingSpaceService = workingSpaceService;
        }

        [HttpGet("GetAll")]
        public async Task<IActionResult> GetAll()
        {
            var data = await workingSpaceService.GetAll();
            if (data == null || data.Count == 0)
            {
                return NotFound();
            }
            return Ok(data);
        }

        [HttpGet("GetById/{workingSpaceId}")]
        public async Task<IActionResult> GetById(string workingSpaceId)
        {
            var data = await workingSpaceService.GetByUserId(workingSpaceId);
            if (data == null)
            {
                return NotFound();
            }
            return Ok(data);
        }

        [HttpPost("Create")]
        public async Task<IActionResult> Create([FromBody] User userAccount)
        {
            var response = await workingSpaceService.CreateUser(userAccount);
            if (response.ResponseCode == 201)
            {
                return CreatedAtAction(nameof(GetById), new { username = userAccount.Username }, response);
            }
            return StatusCode(response.ResponseCode, response);
        }

    }
}
