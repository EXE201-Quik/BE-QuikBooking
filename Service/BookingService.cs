using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Quik_BookingApp.BOs.Request;
using Quik_BookingApp.BOs.Response;
using Quik_BookingApp.DAO;
using Quik_BookingApp.DAO.Models;
using Quik_BookingApp.Helper;
using Quik_BookingApp.Modal;
using Quik_BookingApp.Repos.Interface;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Quik_BookingApp.Service
{
    public class BookingService : IBookingService
    {
        private readonly QuikDbContext _context;
        public readonly IMapper _mapper;
        public readonly ILogger<BookingService> _logger;
        public const decimal CommissionPerPerson = 4000;

        public BookingService(QuikDbContext context, IMapper _mapper, ILogger<BookingService> _logger)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            this._mapper = _mapper ?? throw new ArgumentNullException(nameof(_mapper));
            this._logger = _logger ?? throw new ArgumentNullException(nameof(_logger));
        }

        //can sua lai
        public async Task<APIResponse> BookSpace(BookingRequestModel bookingRequest)
        {
            try
            {
                var space = await _context.WorkingSpaces.FindAsync(bookingRequest.SpaceId);
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

                // Tính tổng chi phí dựa trên giá không gian và thời gian
                var durationInHours = (bookingRequest.EndTime - bookingRequest.StartTime).TotalHours;
                var totalAmount = space.PricePerHour * (decimal)durationInHours;

                // Tạo Booking trước
                var booking = new Booking
                {
                    BookingId = Guid.NewGuid().ToString(),
                    Username = bookingRequest.Username,
                    SpaceId = bookingRequest.SpaceId,
                    StartTime = bookingRequest.StartTime,
                    EndTime = bookingRequest.EndTime,
                    NumberOfPeople = bookingRequest.NumberOfPeople,
                    BookingDate = DateTime.Now,
                    TotalAmount = totalAmount,
                    DepositAmount = CommissionPerPerson * bookingRequest.NumberOfPeople,
                    RemainingAmount = totalAmount - CommissionPerPerson * bookingRequest.NumberOfPeople,
                    Status = "Pending",
                    PaymentId = Guid.NewGuid().ToString(),
                };

                var payment = new Payment
                {
                    PaymentId = booking.PaymentId,
                    BookingId = booking.BookingId,
                    Amount = booking.DepositAmount,
                    PaymentMethod = "VNPay",
                    PaymentDate = DateTime.Now,
                    PaymentStatus = "Pending",
                    VNPayTransactionId = "huyquang",
                    VNPayResponseCode = "huyquang",
                    PaymentUrl = "huyquang"
                };

                _context.Bookings.Add(booking);
                _context.Payments.Add(payment);
                await _context.SaveChangesAsync();

                _logger.LogInformation("Booking created successfully for user {UserId} and space {SpaceId}.", bookingRequest.Username, bookingRequest.SpaceId);

                return new APIResponse
                {
                    ResponseCode = 201,
                    Result = "Success",
                    Message = "Booking created successfully with PaymentId.",
                };
            }
            catch (DbUpdateException dbEx)
            {
                var innerExceptionMessage = dbEx.InnerException != null ? dbEx.InnerException.Message : dbEx.Message;
                _logger.LogError(dbEx, "Database error while creating booking: {Message}", innerExceptionMessage);

                return new APIResponse
                {
                    ResponseCode = 500,
                    Result = "Failed",
                    Message = "Database error: " + innerExceptionMessage
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while creating the booking.");

                return new APIResponse
                {
                    ResponseCode = 500,
                    Result = "Failed",
                    Message = "An unexpected error occurred: " + ex.Message
                };
            }
        }

        
        public async Task<BusinessResponseModel> GetBookingById(string bookingId)
        {
            try
            {
                var booking = await _context.Bookings.FindAsync(bookingId);
                if (booking == null)
                {
                    return null; 
                }
                var bookingModal = _mapper.Map<BusinessResponseModel>(booking); 
                return bookingModal;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving booking by ID."); 
                return null;
            }
        }

        
        public async Task<List<BookingResponseModel>> GetAllBookings()
        {
            List<BookingResponseModel> _response = new List<BookingResponseModel>();
            var _data = await _context.Bookings.ToListAsync(); 
            if (_data != null)
            {
                _response = _mapper.Map<List<Booking>, List<BookingResponseModel>>(_data); 
            }
            return _response;
        }

        
        public async Task<BusinessResponseModel> UpdateBooking(string bookingId, BookingResponseModel bookingRequest)
        {
            try
            {
                var booking = await _context.Bookings.FindAsync(bookingId); 
                if (booking == null)
                {
                    throw new Exception("Booking not found."); 
                }

                var workingSpace = await _context.WorkingSpaces.FindAsync(bookingRequest.SpaceId); 
                if (workingSpace == null)
                {
                    throw new Exception("Working space not found.");
                }

                booking.SpaceId = bookingRequest.SpaceId;
                booking.StartTime = bookingRequest.StartTime;
                booking.EndTime = bookingRequest.EndTime;
                booking.TotalAmount = (booking.EndTime - booking.StartTime).Hours * workingSpace.PricePerHour; 

                
                _context.Bookings.Update(booking);
                await _context.SaveChangesAsync();

                var business = await _context.Businesses.FindAsync(workingSpace.BusinessId);
                if (business == null)
                {
                    throw new Exception("Business not found.");
                }

                var businessResponse = _mapper.Map<BusinessResponseModel>(business); 

                return businessResponse;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while updating the booking.");
                throw; 
            }
        }

        // Xóa đặt chỗ
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

                _context.Bookings.Remove(booking); // Xóa đặt chỗ khỏi cơ sở dữ liệu
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
