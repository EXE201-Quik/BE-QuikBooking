using AutoMapper;
using Azure;
using Microsoft.EntityFrameworkCore;
using Quik_BookingApp.Helper;
using Quik_BookingApp.Models;
using Quik_BookingApp.Repos;
using Quik_BookingApp.Repos.Response;
using Quik_BookingApp.Service;

namespace Quik_BookingApp.Container
{
    public class BusinessService : IBusinessService
    {
        private readonly QuikDbContext _context;
        public readonly IMapper _mapper;
        public readonly ILogger<BusinessService> _logger;

        public BusinessService(QuikDbContext context, IMapper _mapper, ILogger<BusinessService> _logger)
        {
            this._context = context;
            this._mapper = _mapper;
            this._logger = _logger;
        }

        public async Task<List<BusinessResponseModel>> GetAllBusiness()
        {
            List<BusinessResponseModel> _response = new List<BusinessResponseModel>();
            var _data = await this._context.Businesses.ToListAsync();
            if (_data != null)
            {
                _response = this._mapper.Map<List<Business>, List<BusinessResponseModel>>(_data);
            }
            return _response;
        }

        public async Task<APIResponse> GetBusinessById(string bid)
        {
            try
            {
                var business = await _context.Businesses.FindAsync(bid);
                if (business == null)
                {
                    return new APIResponse
                    {
                        ResponseCode = 404,
                        Result = "Failed",
                        Message = "Booking not found."
                    };
                }

                var businessResponse = _mapper.Map<BusinessResponseModel>(bid);
                return new APIResponse
                {
                    ResponseCode = 200,
                    Result = "Success",
                    Message = "Booking retrieved successfully.",
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while retrieving the booking.");
                return new APIResponse
                {
                    ResponseCode = 500,
                    Result = "Failed",
                    Message = "Error occurred while retrieving the booking.",
                };
            }
        }

        
    }
}
