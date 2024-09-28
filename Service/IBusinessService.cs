using Quik_BookingApp.Helper;
using Quik_BookingApp.Repos.Response;

namespace Quik_BookingApp.Service
{
    public interface IBusinessService
    {
        Task<List<BusinessResponseModel>> GetAllBusiness();
        Task<APIResponse> GetBusinessById(string bid);
    }
}
