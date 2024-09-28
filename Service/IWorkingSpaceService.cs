using Quik_BookingApp.Helper;
using Quik_BookingApp.Models;
using Quik_BookingApp.Repos.Request;
using Quik_BookingApp.Repos.Response;

namespace Quik_BookingApp.Service
{
    public interface IWorkingSpaceService
    {
        Task<List<WorkingSpaceRequestModel>> GetAll();
        Task<WorkingSpaceRequestModel> GetBySpaceId(string spaceId);
        Task<APIResponse> CreateWS(WorkingSpaceRequestModel ws);
    }
}
