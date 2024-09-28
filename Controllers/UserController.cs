using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Quik_BookingApp.Service;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Quik_BookingApp.Models;
using Quik_BookingApp.Repos.Request;


namespace Quik_BookingApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUserService userService;
        private readonly IEmailService emailService;

        public UserController(IUserService userService, IEmailService emailService)
        {
            this.userService = userService;
            this.emailService = emailService;
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

        [HttpGet("GetById/{username}")]
        public async Task<IActionResult> GetById(string username)
        {
            var data = await userService.GetByUserId(username);
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

        [HttpPost("UserRegisteration")]
        public async Task<IActionResult> UserRegisteration(UserRegister userRegister)
        {
            var data = await this.userService.UserRegisteration(userRegister);
            return Ok(data);
        }

        [HttpPost("ConfirmRegisteration")]
        public async Task<IActionResult> ConfirmRegisteration(string userid, string username, string otptext)
        {
            var data = await this.userService.ConfirmRegister(userid, username, otptext);
            return Ok(data);
        }

        [HttpPost("ResetPassword")]
        public async Task<IActionResult> ResetPassword(string username, string oldpassword, string newpassword)
        {
            var data = await this.userService.ResetPassword(username, oldpassword, newpassword);
            return Ok(data);
        }

        [HttpPost("ForgetPassword")]
        public async Task<IActionResult> ForgetPassword(string username)
        {
            var data = await this.userService.ForgetPassword(username);
            return Ok(data);
        }

        [HttpPost("UpdatePassword")]
        public async Task<IActionResult> UpdatePassword(string username, string password, string otptext)
        {
            var data = await this.userService.UpdatePassword(username, password, otptext);
            return Ok(data);
        }

        [HttpPost("UpdateStatus")]
        public async Task<IActionResult> UpdateStatus(string username, string status)
        {
            var data = await this.userService.UpdateStatus(username, status);
            return Ok(data);
        }

        [HttpPost("UpdateRole")]
        public async Task<IActionResult> UpdateRole(string username, string role)
        {
            var data = await this.userService.UpdateRole(username, role);
            return Ok(data);
        }

        [HttpPost("SendMail")]
        public async Task<IActionResult> SendMail()
        {
            try
            {
                MailRequest mailrequest = new MailRequest();
                mailrequest.ToEmail = "anchorle3543@gmail.com";
                mailrequest.Subject = "Welcome to Huy world";
                mailrequest.Body = GetHtmlcontent();
                await emailService.SendEmailAsync(mailrequest);
                return Ok();
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        private string GetHtmlcontent()
        {
            string Response = "<div style=\"width:100%;background-color:lightblue;text-align:center;margin:10px\">";
            Response += "<h1>Welcome to QuanG HuY</h1>";
            Response += "<img src=\"https://drive.google.com/file/d/1OAcGjE-e5_4BnxdqMuM4Ezw8pEIqP0VZ/view?usp=sharing\" />";
            Response += "<h2>Thanks for see me <3</h2>";
            Response += "<a href=\"https://drive.google.com/file/d/1OAcGjE-e5_4BnxdqMuM4Ezw8pEIqP0VZ/view?usp=sharing\">Please join membership by click the link</a>";
            Response += "<div><h1> Contact us : anchorle3543@gmail.com</h1></div>";
            Response += "</div>";
            return Response;
        }
    }
}
