using AutoMapper;
using Azure;
using Microsoft.EntityFrameworkCore;
using Quik_BookingApp.BOs.Response;
using Quik_BookingApp.DAO;
using Quik_BookingApp.DAO.Models;
using Quik_BookingApp.Helper;
using Quik_BookingApp.Repos.Interface;

namespace Quik_BookingApp.Service
{
    public class BusinessService : IBusinessService
    {
        private readonly QuikDbContext _context;
        public readonly IMapper _mapper;
        public readonly ILogger<BusinessService> _logger;

        public BusinessService(QuikDbContext context, IMapper _mapper, ILogger<BusinessService> _logger)
        {
            _context = context;
            this._mapper = _mapper;
            this._logger = _logger;
        }

        public async Task<List<BusinessResponseModel>> GetAllBusiness()
        {
            List<BusinessResponseModel> _response = new List<BusinessResponseModel>();
            var _data = await _context.Businesses.ToListAsync();
            if (_data != null)
            {
                _response = _mapper.Map<List<Business>, List<BusinessResponseModel>>(_data);
            }
            return _response;
        }

        public async Task<BusinessResponseModel> GetBusinessById(string bid)
        {
            try
            {
                // Find the business by its ID
                var business = await _context.Businesses.FindAsync(bid);
                if (business == null)
                {
                    throw new Exception("Business not found.");
                }

                // Map the business entity to BusinessResponseModel
                var businessResponse = _mapper.Map<BusinessResponseModel>(business);

                return businessResponse;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while retrieving the business.");
                throw; // Rethrow the exception so it can be handled by the calling method
            }
        }



    }
}
