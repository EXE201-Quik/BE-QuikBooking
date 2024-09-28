using Quik_BookingApp.Helper;
using Quik_BookingApp.Modal;
using Quik_BookingApp.Models;
using Quik_BookingApp.Repos.Request;


namespace Quik_BookingApp.Service
{
    public interface IUserService
    {
        Task<List<UserModal>> GetAll();
        Task<UserModal> GetByUserId(string username);
        Task<APIResponse> CreateUser(User user);
        Task<APIResponse> ConfirmRegister(string userid, string username, string otptext);
        Task<APIResponse> UserRegisteration(UserRegister userRegister);
        Task<APIResponse> ResetPassword(string username, string oldpassword, string newpassword);
        Task<APIResponse> ForgetPassword(string username);
        Task<APIResponse> UpdatePassword(string username, string Password, string Otptext);
        //Task<APIResponse> UpdateOtp(string username, string otpText, string otpType);
        //Task<bool> ValidateOTP(string username, string OTPText);
        //Task UpdatePWDManager(string username, string password);
        //Task<bool> Validatepwdhistory(string Username, string password);
        Task<APIResponse> UpdateStatus(string name, string userstatus);
        Task<APIResponse> UpdateRole(string name, string userrole);
    }
}
