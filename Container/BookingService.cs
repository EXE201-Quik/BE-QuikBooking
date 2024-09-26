using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Quik_BookingApp.Helper;
using Quik_BookingApp.Modal;
using Quik_BookingApp.Models;
using Quik_BookingApp.Repos;
using Quik_BookingApp.Repos.Request;
using Quik_BookingApp.Repos.Response;
using Quik_BookingApp.Service;

namespace Quik_BookingApp.Container
{
    public class BookingService : IBookingService
    {
        private readonly QuikDbContext _context;
        public readonly IMapper _mapper;
        public readonly ILogger<BookingService> _logger;

        public BookingService(QuikDbContext context,IMapper _mapper, ILogger<BookingService> _logger)
        {
            this._context = context;
            this._mapper = _mapper;
            this._logger = _logger;
        }

        public async Task<APIResponse> BookSpace(BookingRequestModel bookingRequest)
        {
            try
            {
                // Check if the working space exists
                var space = _context.WorkingSpaces.Find(bookingRequest.SpaceId);
                if (space == null)
                {
                    _logger.LogWarning("Working space with ID {SpaceId} not found.", bookingRequest.SpaceId);
                    return new APIResponse
                    {
                        ResponseCode = 404,
                        Result = "Failed",
                        Message = "Working space not found.",
                    };
                }

                // Create new booking
                var booking = new Booking
                {
                    BookingId = Guid.NewGuid().ToString(),
                    Username = bookingRequest.Username,
                    SpaceId = bookingRequest.SpaceId,
                    StartTime = bookingRequest.StartTime,
                    EndTime = bookingRequest.EndTime,
                    BookingDate = DateTime.Now,
                    TotalAmount = (bookingRequest.EndTime - bookingRequest.StartTime).Hours * space.PricePerHour,
                    Status = "Pending"
                };

                // Add booking to the database
                _context.Bookings.Add(booking);
                _context.SaveChanges();

                // Map the booking entity to the response model (if required)
                var bookingResponse = _mapper.Map<BookingResponseModel>(booking);

                _logger.LogInformation("Booking created successfully for user {UserId} and space {SpaceId}.", bookingRequest.Username, bookingRequest.SpaceId);

                return new APIResponse
                {
                    ResponseCode = 201,
                    Result = "Success",
                    Message = "Booking created successfully",
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while creating a booking.");
                return new APIResponse
                {
                    ResponseCode = 500,
                    Result = "Failed",
                    Message = "Error occurred while creating the booking.",
                };
            }
        }

        public async Task<APIResponse> GetBookingById(string bookingId)
        {
            try
            {
                var booking = await _context.Bookings.FindAsync(bookingId);
                if (booking == null)
                {
                    return new APIResponse
                    {
                        ResponseCode = 404,
                        Result = "Failed",
                        Message = "Booking not found."
                    };
                }

                var bookingResponse = _mapper.Map<BookingResponseModel>(booking);
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

        public async Task<List<BookingResponseModel>> GetAllBookings()
        {
            List<BookingResponseModel> _response = new List<BookingResponseModel>();
            var _data = await this._context.Bookings.ToListAsync();
            if (_data != null)
            {
                _response = this._mapper.Map<List<Booking>, List<BookingResponseModel>>(_data);
            }
            return _response;
        }

        public async Task<APIResponse> UpdateBooking(string bookingId, Booking bookingRequest)
        {
            try
            {
                var booking = await _context.Bookings.FindAsync(bookingId);
                if (booking == null)
                {
                    return new APIResponse
                    {
                        ResponseCode = 404,
                        Result = "Failed",
                        Message = "Booking not found."
                    };
                }

                // Update booking properties
                booking.SpaceId = bookingRequest.SpaceId;
                booking.StartTime = bookingRequest.StartTime;
                booking.EndTime = bookingRequest.EndTime;
                booking.TotalAmount = (bookingRequest.EndTime - bookingRequest.StartTime).Hours * (await _context.WorkingSpaces.FindAsync(bookingRequest.SpaceId)).PricePerHour;

                _context.Bookings.Update(booking);
                await _context.SaveChangesAsync();

                var bookingResponse = _mapper.Map<BookingResponseModel>(booking);
                return new APIResponse
                {
                    ResponseCode = 200,
                    Result = "Success",
                    Message = "Booking updated successfully.",
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while updating the booking.");
                return new APIResponse
                {
                    ResponseCode = 500,
                    Result = "Failed",
                    Message = "Error occurred while updating the booking.",
                };
            }
        }

        public async Task<APIResponse> DeleteBooking(string bookingId)
        {
            try
            {
                var booking = await _context.Bookings.FindAsync(bookingId);
                if (booking == null)
                {
                    return new APIResponse
                    {
                        ResponseCode = 404,
                        Result = "Failed",
                        Message = "Booking not found."
                    };
                }

                _context.Bookings.Remove(booking);
                await _context.SaveChangesAsync();

                return new APIResponse
                {
                    ResponseCode = 204,
                    Result = "Success",
                    Message = "Booking deleted successfully."
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while deleting the booking.");
                return new APIResponse
                {
                    ResponseCode = 500,
                    Result = "Failed",
                    Message = "Error occurred while deleting the booking.",
                };
            }
        }

    }
}


