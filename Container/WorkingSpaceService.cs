using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Quik_BookingApp.Helper;
using Quik_BookingApp.Models;
using Quik_BookingApp.Repos;
using Quik_BookingApp.Repos.Request;
using Quik_BookingApp.Repos.Response;
using Quik_BookingApp.Service;

namespace Quik_BookingApp.Container
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
            this._logger = logger;
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

                var workingSpace = new WorkingSpace
                {
                    SpaceId = Guid.NewGuid().ToString(),
                    BusinessId = ws.BusinessId,
                    ImageId = ws.ImageId,
                    Title = ws.Title,
                    Description = ws.Description,
                    PricePerHour = ws.PricePerHour,
                    Capacity = ws.Capacity,
                    Location = ws.Location,
                    Bookings = new List<Booking>(),
                };

                // Add the user to the database
                await context.WorkingSpaces.AddAsync(workingSpace);
                await context.SaveChangesAsync();

                // Create a response with the newly created user
                var workingSpaceModal = mapper.Map<WorkingSpaceResponse>(workingSpace);

                return new APIResponse
                {
                    ResponseCode = 201,
                    Result = "Success",
                    Message = "Working space created successfully.",
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating working space.");
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
            var _data = await this.context.WorkingSpaces.ToListAsync();
            if (_data != null)
            {
                _response = this.mapper.Map<List<WorkingSpace>, List<WorkingSpaceRequestModel>>(_data);
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
                _logger.LogError(ex, "Error retrieving user by ID.");
                return null;
            }
        }
    }
}
