using Quik_BookingApp.BOs.Response;
using Quik_BookingApp.DAO.Models;
using Quik_BookingApp.Helper;

namespace Quik_BookingApp.Repos.Interface
{
    public interface IBusinessService
    {
        
            Task<IEnumerable<Business>> GetAllBusinessesAsync();
            Task<Business> GetBusinessByIdAsync(string businessId);
            Task<Business> CreateBusinessAsync(Business business);
            Task<Business> UpdateBusinessAsync(Business business);
            Task<bool> DeleteBusinessAsync(string businessId);
        

    }
}
