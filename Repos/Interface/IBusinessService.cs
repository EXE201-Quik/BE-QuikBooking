using Quik_BookingApp.BOs.Response;
using Quik_BookingApp.Helper;

namespace Quik_BookingApp.Repos.Interface
{
    public interface IBusinessService
    {
        Task<List<BusinessResponseModel>> GetAllBusiness();
        Task<List<WorkingSpaceResponse>> GetListWSOfBusiness(string businessId);
        Task<BusinessResponseModel> GetBusinessById(string bid);
    }
}
