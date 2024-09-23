using Quik_BookingApp.Helper;
using Quik_BookingApp.Modal;
using Quik_BookingApp.Models;


namespace Quik_BookingApp.Service
{
    public interface IUserService
    {
        Task<List<UserModal>> GetAll();
        Task<UserModal> GetByUserId(string userId);
        Task<APIResponse> CreateUser(User user);
        //Task<APIResponse> UserRegisteration(UserRegister userRegister);
        
        //Task<APIResponse> ConfirmRegister(int userid, string username, string otptext);
        //Task<APIResponse> ResetPassword(string username, string oldpassword, string newpassword);
        //Task<APIResponse> ForgetPassword(string username);
        //Task<APIResponse> UpdatePassword(string username, string Password, string Otptext);
        Task<APIResponse> UpdateStatus(string name, string userstatus);
        Task<APIResponse> UpdateRole(string name, string userrole);
    }
}
