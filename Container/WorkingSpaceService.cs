using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Quik_BookingApp.Helper;
using Quik_BookingApp.Modal;
using Quik_BookingApp.Models;
using Quik_BookingApp.Repos;
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

        public async Task<APIResponse> CreateUser(User user)
        {
            try
            {
                if (user == null)
                {
                    return new APIResponse
                    {
                        ResponseCode = 400,
                        Result = "Failure",
                        Message = "User cannot be null"
                    };
                }

                // Add the user to the database
                await context.Users.AddAsync(user);
                await context.SaveChangesAsync();

                // Create a response with the newly created user
                var workingSpaceModal = mapper.Map<WorkingSpaceModal>(user);

                return new APIResponse
                {
                    ResponseCode = 201,
                    Result = "Success",
                    Message = "User created successfully.",
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating user.");
                return new APIResponse
                {
                    ResponseCode = 500,
                    Result = "Failure",
                    Message = "Error creating user."
                };
            }
        }


        public async Task<List<WorkingSpaceModal>> GetAll()
        {
            List<WorkingSpaceModal> _response = new List<WorkingSpaceModal>();
            var _data = await this.context.WorkingSpaces.ToListAsync();
            if (_data != null)
            {
                _response = this.mapper.Map<List<WorkingSpace>, List<WorkingSpaceModal>>(_data);
            }
            return _response;
        }

        public async Task<WorkingSpaceModal> GetByUserId(string workingSpaceId)
        {
            try
            {
                var workingSpace = await context.WorkingSpaces.FindAsync(workingSpaceId);
                if (workingSpace == null)
                {
                    return null;
                }
                var workingSpaceModal = mapper.Map<WorkingSpaceModal>(workingSpace);
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
