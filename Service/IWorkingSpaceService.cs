using Quik_BookingApp.Helper;
using Quik_BookingApp.Modal;
using Quik_BookingApp.Models;

namespace Quik_BookingApp.Service
{
    public interface IWorkingSpaceService
    {
        Task<List<WorkingSpaceModal>> GetAll();
        Task<WorkingSpaceModal> GetByUserId(string userId);
        Task<APIResponse> CreateUser(User user);
    }
}
