using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Quik_BookingApp.Service;
using Quik_BookingApp.Modal;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Quik_BookingApp.Models;


namespace Quik_BookingApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUserService userService;

        public UserController(IUserService userService)
        {
            this.userService = userService;
        }

        [HttpGet("GetAll")]
        public async Task<IActionResult> GetAll()
        {
            var data = await userService.GetAll();
            if (data == null || data.Count == 0)
            {
                return NotFound();
            }
            return Ok(data);
        }

        [HttpGet("GetById/{userId}")]
        public async Task<IActionResult> GetById(int userId)
        {
            var data = await userService.GetByUserId(userId);
            if (data == null)
            {
                return NotFound();
            }
            return Ok(data);
        }

        [HttpPost("Create")]
        public async Task<IActionResult> Create([FromBody] User userAccount)
        {
            var response = await userService.CreateUser(userAccount);
            if (response.ResponseCode == 201)
            {
                return CreatedAtAction(nameof(GetById), new { username = userAccount.Username }, response);
            }
            return StatusCode(response.ResponseCode, response);
        }

        [HttpPost("userregisteration")]
        public async Task<IActionResult> UserRegisteration(UserRegister userRegister)
        {
            var data = await this.userService.UserRegisteration(userRegister);
            return Ok(data);
        }

        [HttpPost("confirmregisteration")]
        public async Task<IActionResult> ConfirmRegisteration(int userid, string username, string otptext)
        {
            var data = await this.userService.ConfirmRegister(userid, username, otptext);
            return Ok(data);
        }

        [HttpPost("resetpassword")]
        public async Task<IActionResult> ResetPassword(string username, string oldpassword, string newpassword)
        {
            var data = await this.userService.ResetPassword(username, oldpassword, newpassword);
            return Ok(data);
        }

        [HttpPost("forgetpassword")]
        public async Task<IActionResult> ForgetPassword(string username)
        {
            var data = await this.userService.ForgetPassword(username);
            return Ok(data);
        }

        [HttpPost("updatepassword")]
        public async Task<IActionResult> UpdatePassword(string username, string password, string otptext)
        {
            var data = await this.userService.UpdatePassword(username, password, otptext);
            return Ok(data);
        }

        [HttpPost("updatestatus")]
        public async Task<IActionResult> UpdateStatus(string username, string status)
        {
            var data = await this.userService.UpdateStatus(username, status);
            return Ok(data);
        }

        [HttpPost("updaterole")]
        public async Task<IActionResult> UpdateRole(string username, string role)
        {
            var data = await this.userService.UpdateRole(username, role);
            return Ok(data);
        }
    }
}
