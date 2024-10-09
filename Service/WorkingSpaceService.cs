using AutoMapper;
using Azure;
using Microsoft.EntityFrameworkCore;
using Quik_BookingApp.BOs.Request;
using Quik_BookingApp.BOs.Response;
using Quik_BookingApp.DAO;
using Quik_BookingApp.DAO.Models;
using Quik_BookingApp.Helper;
using Quik_BookingApp.Repos.Interface;
using FirebaseAdmin;
using Google.Apis.Auth.OAuth2;


namespace Quik_BookingApp.Service
{
    public class WorkingSpaceService : IWorkingSpaceService
    {
        public readonly QuikDbContext context;
        public readonly IMapper mapper;
        public readonly ILogger<WorkingSpaceService> _logger;

        public WorkingSpaceService(QuikDbContext context, IMapper mapper, ILogger<WorkingSpaceService> logger)
        {
            this.context = context;
            this.mapper = mapper;
            _logger = logger;
        }

        public async Task<APIResponse> CreateWS(WorkingSpaceRequestModel ws)
        {
            try
            {
                if (ws == null)
                {
                    return new APIResponse
                    {
                        ResponseCode = 400,
                        Result = "Failure",
                        Message = "Working space cannot be null"
                    };
                }

                // Log the BusinessId being used
                _logger.LogInformation("Attempting to retrieve business with ID: {BusinessId}", ws.BusinessId);

                // Retrieve the business first
                var business = await context.Businesses.FindAsync(ws.BusinessId);
                if (business == null)
                {
                    return new APIResponse
                    {
                        ResponseCode = 400,
                        Result = "Failure",
                        Message = "Business not found."
                    };
                }

                var workingSpace = new WorkingSpace
                {
                    SpaceId = Guid.NewGuid().ToString(),
                    BusinessId = business.BusinessId,
                    ImageId = ws.ImageId,
                    Title = ws.Title,
                    Description = ws.Description,
                    PricePerHour = ws.PricePerHour,
                    Capacity = ws.Capacity,
                    Location = ws.Location,
                    Bookings = new List<Booking>(),
                };

                // Log the new working space creation details
                _logger.LogInformation("Creating working space: {Title} for Business ID: {BusinessId}", workingSpace.Title, workingSpace.BusinessId);

                // Add the working space to the database
                await context.WorkingSpaces.AddAsync(workingSpace);
                await context.SaveChangesAsync();

                // Map to WorkingSpaceModel
                var workingSpaceModel = mapper.Map<WorkingSpace>(workingSpace);

                return new APIResponse
                {
                    ResponseCode = 201,
                    Result = "Success",
                    Message = "Working space created successfully.",
                };
            }
            catch (DbUpdateException dbEx)
            {
                var innerExceptionMessage = dbEx.InnerException != null ? dbEx.InnerException.Message : dbEx.Message;
                _logger.LogError(dbEx, "Database error during working space creation: {Message}", innerExceptionMessage);

                return new APIResponse
                {
                    ResponseCode = 500,
                    Result = "Failure",
                    Message = "Database error: " + innerExceptionMessage
                };
            }
            catch (Exception ex)
            {
                // Log detailed exception information
                _logger.LogError(ex, "Error creating working space: {Message}", ex.ToString());
                return new APIResponse
                {
                    ResponseCode = 500,
                    Result = "Failure",
                    Message = "Error creating working space."
                };
            }
        }




        public async Task<List<WorkingSpaceRequestModel>> GetAll()
        {
            List<WorkingSpaceRequestModel> _response = new List<WorkingSpaceRequestModel>();
            var _data = await context.WorkingSpaces.ToListAsync();
            if (_data != null)
            {
                _response = mapper.Map<List<WorkingSpace>, List<WorkingSpaceRequestModel>>(_data);
            }
            return _response;
        }

        public async Task<WorkingSpaceRequestModel> GetBySpaceId(string spaceId)
        {
            try
            {
                var workingSpace = await context.WorkingSpaces.FindAsync(spaceId);
                if (workingSpace == null)
                {
                    return null;
                }
                var workingSpaceModal = mapper.Map<WorkingSpaceRequestModel>(workingSpace);
                return workingSpaceModal;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving working space by ID.");
                return null;
            }
        }
    }
}
