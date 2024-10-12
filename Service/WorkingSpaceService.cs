﻿using AutoMapper;
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
        public readonly IFirebaseService _firebase;
        public readonly IConfiguration _configuration;

        public WorkingSpaceService(QuikDbContext context, IMapper mapper, ILogger<WorkingSpaceService> logger,IFirebaseService firebaseService, IConfiguration configuration)
        {
            this.context = context;
            this.mapper = mapper;
            this._logger = logger;
            this._firebase = firebaseService ?? throw new ArgumentNullException(nameof(firebaseService));
            this._configuration = configuration;
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

                // Create a new working space instance
                var workingSpace = new WorkingSpace
                {
                    SpaceId = Guid.NewGuid().ToString(),
                    BusinessId = business.BusinessId,
                    ImageId = Guid.NewGuid().ToString(),
                    Title = ws.Title,
                    Description = ws.Description,
                    PricePerHour = ws.PricePerHour,
                    Capacity = ws.Capacity,
                    Location = ws.Location,
                    RoomType = ws.RoomType,
                    Bookings = new List<Booking>(),
                    Images = new List<ImageWS>() // Initialize Images list
                };

                // Check if an image is uploaded
                if (ws.Image != null && ws.Image.Length > 0)
                {
                    // Upload the image to Firebase
                    var uploadResult = await _firebase.UploadFileToFirebase(ws.Image, $"workingspaces/{ws.Title}_{DateTime.Now.Ticks}");

                    if (uploadResult.Status == 200)
                    {
                        // Create a new ImageWS object and set the ImageUrl
                        var newImageWS = new ImageWS
                        {
                            ImageUrl = uploadResult.Data.ToString(), 
                        };
                        workingSpace.Images.Add(newImageWS); 

                        _logger.LogInformation("Image uploaded successfully to Firebase for working space: {Title}", ws.Title);
                    }
                    else
                    {
                        return new APIResponse
                        {
                            ResponseCode = 500,
                            Result = "Failure",
                            Message = "Failed to upload image to Firebase."
                        };
                    }
                }
                else
                {
                    // Use a default image if no image is uploaded
                    var defaultImageWS = new ImageWS
                    {
                        ImageUrl = _configuration["DefaultImageUrl"] // This should be configured in appsettings.json or environment variables
                    };
                    workingSpace.Images.Add(defaultImageWS); // Add to Images collection

                    _logger.LogInformation("No image uploaded, using default image for working space: {Title}", ws.Title);
                }

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

        public async Task<WorkingSpaceResponseAmenities> GetBySpaceId(string spaceId)
        {
            try
            {
                // Include amenities when retrieving the working space
                var workingSpace = await context.WorkingSpaces
                                                .Include(ws => ws.Amenities)
                                                .FirstOrDefaultAsync(ws => ws.SpaceId == spaceId);

                if (workingSpace == null)
                {
                    return null;
                }

                // Map the working space and amenities to the response model
                var workingSpaceModel = mapper.Map<WorkingSpaceResponseAmenities>(workingSpace);

                return workingSpaceModel;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving working space by ID.");
                return null;
            }
        }


        public async Task<List<WorkingSpaceRequestModel>> GetWSOfRoomType(string roomType)
        {
            try
            {
                roomType = roomType.Trim();

                if (roomType.Equals("Không gian làm việc", StringComparison.OrdinalIgnoreCase))
                {
                    var workingSpaces = await context.WorkingSpaces
                        .Where(ws => ws.RoomType.Equals("Không gian làm việc", StringComparison.OrdinalIgnoreCase))
                        .ToListAsync();

                    return mapper.Map<List<WorkingSpaceRequestModel>>(workingSpaces);
                }
                else if (roomType.Equals("Phòng họp", StringComparison.OrdinalIgnoreCase))
                {
                    var workingSpaces = await context.WorkingSpaces
                        .Where(ws => ws.RoomType.Equals("Phòng họp", StringComparison.OrdinalIgnoreCase))
                        .ToListAsync();

                    return mapper.Map<List<WorkingSpaceRequestModel>>(workingSpaces);
                }
                else if (roomType.Equals("Không gian chung", StringComparison.OrdinalIgnoreCase))
                {
                    var workingSpaces = await context.WorkingSpaces
                        .Where(ws => ws.RoomType.Equals("Không gian chung", StringComparison.OrdinalIgnoreCase))
                        .ToListAsync();

                    return mapper.Map<List<WorkingSpaceRequestModel>>(workingSpaces);
                }
                else if (roomType.Equals("Văn phòng riêng", StringComparison.OrdinalIgnoreCase))
                {
                    var workingSpaces = await context.WorkingSpaces
                        .Where(ws => ws.RoomType.Equals("Văn phòng riêng", StringComparison.OrdinalIgnoreCase))
                        .ToListAsync();

                    return mapper.Map<List<WorkingSpaceRequestModel>>(workingSpaces);
                }
                else
                {
                    _logger.LogWarning("Room type not recognized: {RoomType}", roomType);
                    return new List<WorkingSpaceRequestModel>();
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving working space by room type.");
                return null;
            }
        }


    }
}
