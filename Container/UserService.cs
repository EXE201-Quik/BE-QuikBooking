using AutoMapper;
using Azure;
using Microsoft.EntityFrameworkCore;
using Quik_BookingApp.Helper;
using Quik_BookingApp.Modal;
using Quik_BookingApp.Models;
using Quik_BookingApp.Repos;
using Quik_BookingApp.Repos.Models;
using Quik_BookingApp.Service;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Quik_BookingApp.Container
{
    public class UserService : IUserService
    {
        public readonly QuikDbContext context;
        public readonly IMapper mapper;
        public readonly ILogger<UserService> _logger;

        public UserService(QuikDbContext context, IMapper mapper, ILogger<UserService> logger)
        {
            this.context = context;
            this.mapper = mapper;
            this._logger = logger;
        }

        public async Task<APIResponse> CreateUser(User user)
        {
            try
            {
                if (user == null)
                {
                    return new APIResponse
                    {
                        ResponseCode = 400,
                        Result = "Failure",
                        Message = "User cannot be null"
                    };
                }

                // Add the user to the database
                await context.Users.AddAsync(user);
                await context.SaveChangesAsync();

                // Create a response with the newly created user
                var userModal = mapper.Map<UserModal>(user);

                return new APIResponse
                {
                    ResponseCode = 201,
                    Result = "Success",
                    Message = "User created successfully.",
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating user.");
                return new APIResponse
                {
                    ResponseCode = 500,
                    Result = "Failure",
                    Message = "Error creating user."
                };
            }
        }

        public async Task<List<UserModal>> GetAll()
        {
            List<UserModal> _response = new List<UserModal>();
            var _data = await this.context.Users.ToListAsync();
            if (_data != null)
            {
                _response = this.mapper.Map<List<User>, List<UserModal>>(_data);
            }
            return _response;
            
        }

        public async Task<UserModal> GetByUserId(int userId)
        {
            try
            {
                UserModal _response = new UserModal();
                var data = await context.Users.FindAsync(userId);
                if (data != null)
                {
                    _response = this.mapper.Map<User,UserModal>(data);
                }
                return _response;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving user by ID.");
                return null;
            }
        }

        public async Task<APIResponse> ConfirmRegister(int userid, string username, string otptext)
        {
            APIResponse response = new APIResponse();
            bool otpresponse = await ValidateOTP(username, otptext);
            if (!otpresponse)
            {
                response.Result = "fail";
                response.Message = "Invalid OTP or Expired";
            }
            else
            {
                var _tempdata = await this.context.Tempusers.FirstOrDefaultAsync(item => item.Id == userid);
                var _user = new User()
                {
                    Name = _tempdata.Name,
                    Password = _tempdata.Password,
                    Email = _tempdata.Email,
                    PhoneNumber = _tempdata.Phone,
                    Role = "user"
                };
                await this.context.Users.AddAsync(_user);
                await this.context.SaveChangesAsync();
                await UpdatePWDManager(username, _tempdata.Password);
                response.Result = "pass";
                response.Message = "Registered successfully.";
            }

            return response;
        }

        public async Task<APIResponse> UserRegisteration(UserRegister userRegister)
        {
            APIResponse response = new APIResponse();
            int userid = 0;
            bool isvalid = true;

            try
            {
                // duplicate user
                var _user = await this.context.Users.Where(item => item.Name == userRegister.Username).ToListAsync();
                if (_user.Count > 0)
                {
                    isvalid = false;
                    response.Result = "fail";
                    response.Message = "duplicate username";
                }

                // duplicate email
                var _useremail = await this.context.Users.Where(item => item.Email == userRegister.Email).ToListAsync();
                if (_useremail.Count > 0)
                {
                    isvalid = false;
                    response.Result = "fail";
                    response.Message = "duplicate email";
                }


                if (userRegister != null && isvalid)
                {
                    var _tempuser = new Tempuser()
                    {
                        Name = userRegister.Name,
                        Email = userRegister.Email,
                        Password = userRegister.Password,
                        Phone = userRegister.PhoneNumber,
                    };
                    await this.context.Tempusers.AddAsync(_tempuser);
                    await this.context.SaveChangesAsync();
                    userid = _tempuser.Id;
                    string otptext = Generaterandomnumber();
                    await UpdateOtp(userRegister.Username, otptext, "register");
                    await SendOtpMail(userRegister.Email, otptext, userRegister.Name);
                    response.Result = "pass";
                    response.Message = userid.ToString();
                }

            }
            catch (Exception ex)
            {
                response.Result = "fail";
            }

            return response;

        }

        public async Task<APIResponse> ResetPassword(string name, string oldpassword, string newpassword)
        {
            APIResponse response = new APIResponse();
            var _user = await this.context.Users.FirstOrDefaultAsync(item => item.Name == name &&
            item.Password == oldpassword && item.Status == "Active");
            if (_user != null)
            {
                var _pwdhistory = await Validatepwdhistory(name, newpassword);
                if (_pwdhistory)
                {
                    response.Result = "fail";
                    response.Message = "Don't use the same password that used in last 3 transaction";
                }
                else
                {
                    _user.Password = newpassword;
                    await this.context.SaveChangesAsync();
                    await UpdatePWDManager(name, newpassword);
                    response.Result = "pass";
                    response.Message = "Password changed.";
                }
            }
            else
            {
                response.Result = "fail";
                response.Message = "Failed to validate old password.";
            }
            return response;
        }

        public async Task<APIResponse> ForgetPassword(string name)
        {
            APIResponse response = new APIResponse();
            var _user = await this.context.Users.FirstOrDefaultAsync(item => item.Name == name && item.Status == "Active");
            if (_user != null)
            {
                string otptext = Generaterandomnumber();
                await UpdateOtp(name, otptext, "forgetpassword");
                await SendOtpMail(_user.Email, otptext, _user.Name);
                response.Result = "pass";
                response.Message = "OTP sent";

            }
            else
            {
                response.Result = "fail";
                response.Message = "Invalid User";
            }
            return response;
        }

        public async Task<APIResponse> UpdatePassword(string name, string Password, string Otptext)
        {
            APIResponse response = new APIResponse();

            bool otpvalidation = await ValidateOTP(name, Otptext);
            if (otpvalidation)
            {
                bool pwdhistory = await Validatepwdhistory(name, Password);
                if (pwdhistory)
                {
                    response.Result = "fail";
                    response.Message = "Don't use the same password that used in last 3 transaction";
                }
                else
                {
                    var _user = await this.context.Users.FirstOrDefaultAsync(item => item.Name == name && item.Status == "Active");
                    if (_user != null)
                    {
                        _user.Password = Password;
                        await this.context.SaveChangesAsync();
                        await UpdatePWDManager(name, Password);
                        response.Result = "pass";
                        response.Message = "Password changed";
                    }
                }
            }
            else
            {
                response.Result = "fail";
                response.Message = "Invalid OTP";
            }
            return response;
        }

        private async Task UpdateOtp(string username, string otptext, string otptype)
        {
            var _opt = new OtpManager()
            {
                Username = username,
                OtpText = otptext,
                Expiration = DateTime.Now.AddMinutes(30),
                CreatedDate = DateTime.Now,
                OtpType = otptype
            };
            await this.context.OtpManagers.AddAsync(_opt);
            await this.context.SaveChangesAsync();
        }

        private async Task<bool> ValidateOTP(string username, string OTPText)
        {
            bool response = false;
            var _data = await this.context.OtpManagers.FirstOrDefaultAsync(item => item.Username == username
            && item.OtpText == OTPText && item.Expiration > DateTime.Now);
            if (_data != null)
            {
                response = true;
            }
            return response;
        }

        private async Task UpdatePWDManager(string username, string password)
        {
            var _opt = new PwdManager()
            {
                Username = username,
                Password = password,
                ModifyDate = DateTime.Now
            };
            await this.context.PwdManagers.AddAsync(_opt);
            await this.context.SaveChangesAsync();
        }

        private string Generaterandomnumber()
        {
            Random random = new Random();
            string randomno = random.Next(0, 1000000).ToString("D6");
            return randomno;
        }

        private async Task SendOtpMail(string useremail, string OtpText, string Name)
        {

        }

        private async Task<bool> Validatepwdhistory(string Username, string password)
        {
            bool response = false;
            var _pwd = await this.context.PwdManagers.Where(item => item.Username == Username).
                OrderByDescending(p => p.ModifyDate).Take(3).ToListAsync();
            if (_pwd.Count > 0)
            {
                var validate = _pwd.Where(o => o.Password == password);
                if (validate.Any())
                {
                    response = true;
                }
            }

            return response;

        }

        public async Task<APIResponse> UpdateStatus(string name, string userstatus)
        {
            APIResponse response = new APIResponse();
            var _user = await this.context.Users.FirstOrDefaultAsync(item => item.Name == name);
            if (_user != null)
            {
                _user.Status = userstatus;
                await this.context.SaveChangesAsync();
                response.Result = "pass";
                response.Message = "User Status changed";
            }
            else
            {
                response.Result = "fail";
                response.Message = "Invalid User";
            }
            return response;
        }

        public async Task<APIResponse> UpdateRole(string name, string userrole)
        {
            APIResponse response = new APIResponse();
            var _user = await this.context.Users.FirstOrDefaultAsync(item => item.Name == name && item.Status == "Active");
            if (_user != null)
            {
                _user.Role = userrole;
                await this.context.SaveChangesAsync();
                response.Result = "pass";
                response.Message = "User Role changed";
            }
            else
            {
                response.Result = "fail";
                response.Message = "Invalid User";
            }
            return response;
        }

    }
}
