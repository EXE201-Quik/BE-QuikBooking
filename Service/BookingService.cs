﻿using AutoMapper;
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
        private readonly EmailService _emailService;
        public readonly ILogger<BookingService> _logger;
        public const decimal CommissionPerPerson = 4000;

        public BookingService(QuikDbContext context, IMapper _mapper, ILogger<BookingService> _logger, EmailService emailService)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            this._mapper = _mapper ?? throw new ArgumentNullException(nameof(_mapper));
            this._logger = _logger ?? throw new ArgumentNullException(nameof(_logger));
            _emailService = emailService;
        }

        //can sua lai
        /*## Payment Revise
        - Được 
        Quy ước sẵn mức phí hoa hồng cho 1 người/ đơn là một mức giá cố định.

        VD: Phí cho 1 người/ đơn = 4k ⇒ Nhóm 5 người/ đơn = 20k (đặt trước)

        - Tiền này được xem là tiền cọc mà khách hàng buộc phải thanh toán để hoàn thành đơn giao dịch đặt trước thành công.
        - Tiền cọc này sẽ là tiền hoa hồng của Quik cần thu từ quán.
        - Khi khách hàng tới WS sẽ thanh toán phần tiền đặt phòng còn lại.

        VD: Đơn của khách sum=200k, vậy thì số tiền khách cần trả là 200k-20k=180k
        */

        public async Task<APIResponseData> BookSpace(BookingRequestModel bookingRequest)
        {
            try
            {
                var space = await _context.WorkingSpaces.FindAsync(bookingRequest.SpaceId);
                if (space == null)
                {
                    _logger.LogWarning("Working space with ID {SpaceId} not found.", bookingRequest.SpaceId);
                    return new APIResponseData
                    {
                        ResponseCode = 404,
                        Result = "Failed",
                        Message = "Working space not found.",
                        
                    };
                }

                var user = await _context.Users.FirstOrDefaultAsync(u => u.Username == bookingRequest.Username);
                if (user == null)
                {
                    _logger.LogWarning("User with username {Username} not found.", bookingRequest.Username);
                    return new APIResponseData
                    {
                        ResponseCode = 404,
                        Result = "Failed",
                        Message = "User not found.",
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
                    Status = "Sắp tới",
                    //PaymentId = Guid.NewGuid().ToString(),
                   
                };

                _context.Bookings.Add(booking);
                //_context.Payments.Add(payment);
                await _context.SaveChangesAsync();

                string subject = "Xác nhận đặt phòng thành công";
                string body = $@"
                    <h2>Cảm ơn bạn đã đặt phòng với Quik Booking!</h2>
                    <p>Chi tiết đặt phòng của bạn:</p>
                    <ul>
                        <li>Mã đặt phòng: {booking.BookingId}</li>
                        <li>Không gian: {space.Description}</li>
                        <li>Thời gian: {booking.StartTime} đến {booking.EndTime}</li>
                        <li>Tổng chi phí: {totalAmount} VND</li>
                        <li>Tiền cần thanh toán: {booking.RemainingAmount} VND</li>
                    </ul>";

                await _emailService.SendEmailAsync(user.Email, subject, body);

                _logger.LogInformation("Booking created successfully for user {UserId} and space {SpaceId}.", user.Email, bookingRequest.SpaceId);

                var bookingResponse = new BookingResponseModel
                {
                    BookingId = booking.BookingId,
                    Username = booking.Username,
                    SpaceId = booking.SpaceId,
                    StartTime = booking.StartTime,
                    EndTime = booking.EndTime,
                    NumberOfPeople = booking.NumberOfPeople,
                    BookingDate = booking.BookingDate,
                    TotalAmount = booking.TotalAmount,
                    DepositAmount = booking.DepositAmount,
                    RemainingAmount = booking.RemainingAmount,
                    Status = booking.Status,
                    Location = space.Location,
                    PaymentId = Guid.NewGuid(),
                };

                return new APIResponseData
                {
                    ResponseCode = 201,
                    Result = "Success",
                    Message = "Booking created successfully with PaymentId.",
                    Data = bookingResponse
                };
            }
            catch (DbUpdateException dbEx)
            {
                var innerExceptionMessage = dbEx.InnerException != null ? dbEx.InnerException.Message : dbEx.Message;
                _logger.LogError(dbEx, "Database error while creating booking: {Message}", innerExceptionMessage);

                return new APIResponseData
                {
                    ResponseCode = 500,
                    Result = "Failed",
                    Message = "Database error: " + innerExceptionMessage,
                    Data = dbEx
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while creating the booking.");

                return new APIResponseData
                {
                    ResponseCode = 500,
                    Result = "Failed",
                    Message = "An unexpected error occurred: " + ex.Message,
                    Data = ex
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

        public async Task<List<BookingResponseModel>> GetBookingOfHoanTat(string username)
        {
            try
            {
                // Retrieve the list of bookings for the specified user with status 'Hoàn Tất'
                var bookings = await _context.Bookings
                    .Where(b => b.Username == username && b.Status.Equals("Hoàn Tất"))
                    .ToListAsync();

                if (bookings == null || !bookings.Any())
                {
                    throw new Exception("No completed bookings found for this user.");
                }

                var bookingResponseList = new List<BookingResponseModel>();

                foreach (var booking in bookings)
                {
                    // Fetch the WorkingSpace associated with each booking
                    var workingSpace = await _context.WorkingSpaces.FindAsync(booking.SpaceId);
                    if (workingSpace == null)
                    {
                        throw new Exception($"Working space with ID {booking.SpaceId} not found.");
                    }

                    // Map the booking and working space data into BookingResponseModel
                    var bookingResponse = new BookingResponseModel
                    {
                        BookingId = booking.BookingId,
                        Username = booking.Username,
                        SpaceId = booking.SpaceId,
                        PaymentId = booking.PaymentId,
                        BookingDate = booking.BookingDate,
                        StartTime = booking.StartTime,
                        EndTime = booking.EndTime,
                        NumberOfPeople = booking.NumberOfPeople,
                        TotalAmount = booking.TotalAmount,
                        DepositAmount = booking.DepositAmount,
                        RemainingAmount = booking.RemainingAmount,
                        Status = booking.Status,
                        Location = workingSpace.Location 
                    };

                    bookingResponseList.Add(bookingResponse);
                }

                return bookingResponseList;
            }
            catch (Exception ex)
            {
                // Log the exception and rethrow it
                throw new Exception("An error occurred while retrieving completed bookings: " + ex.Message);
            }
        }

        public async Task<List<BookingResponseModel>> GetBookingOfSapToi(string username)
        {
            try
            {
                // Retrieve the list of bookings for the specified user with status 'Hoàn tất'
                var bookings = await _context.Bookings
                    .Where(b => b.Username == username && b.Status.Equals("Sắp tới"))
                    .ToListAsync();

                if (bookings == null || !bookings.Any())
                {
                    throw new Exception("No completed bookings found for this user.");
                }

                var bookingResponseList = new List<BookingResponseModel>();

                foreach (var booking in bookings)
                {
                    // Fetch the WorkingSpace associated with each booking
                    var workingSpace = await _context.WorkingSpaces.FindAsync(booking.SpaceId);
                    if (workingSpace == null)
                    {
                        throw new Exception($"Working space with ID {booking.SpaceId} not found.");
                    }

                    // Map the booking and working space data into BookingResponseModel
                    var bookingResponse = new BookingResponseModel
                    {
                        BookingId = booking.BookingId,
                        Username = booking.Username,
                        SpaceId = booking.SpaceId,
                        PaymentId = booking.PaymentId,
                        BookingDate = booking.BookingDate,
                        StartTime = booking.StartTime,
                        EndTime = booking.EndTime,
                        NumberOfPeople = booking.NumberOfPeople,
                        TotalAmount = booking.TotalAmount,
                        DepositAmount = booking.DepositAmount,
                        RemainingAmount = booking.RemainingAmount,
                        Status = booking.Status,
                        Location = workingSpace.Location
                    };

                    bookingResponseList.Add(bookingResponse);
                }

                return bookingResponseList;
            }
            catch (Exception ex)
            {
                // Log the exception and rethrow it
                throw new Exception("An error occurred while retrieving completed bookings: " + ex.Message);
            }
        }

        public async Task<List<BookingResponseModel>> GetBookingOfDaHuy(string username)
        {
            try
            {
                // Retrieve the list of bookings for the specified user with status 'Hoàn tất'
                var bookings = await _context.Bookings
                    .Where(b => b.Username == username && b.Status.Equals("Đã hủy"))
                    .ToListAsync();

                if (bookings == null || !bookings.Any())
                {
                    throw new Exception("No completed bookings found for this user.");
                }

                var bookingResponseList = new List<BookingResponseModel>();

                foreach (var booking in bookings)
                {
                    // Fetch the WorkingSpace associated with each booking
                    var workingSpace = await _context.WorkingSpaces.FindAsync(booking.SpaceId);
                    if (workingSpace == null)
                    {
                        throw new Exception($"Working space with ID {booking.SpaceId} not found.");
                    }

                    // Map the booking and working space data into BookingResponseModel
                    var bookingResponse = new BookingResponseModel
                    {
                        BookingId = booking.BookingId,
                        Username = booking.Username,
                        SpaceId = booking.SpaceId,
                        PaymentId = booking.PaymentId,
                        BookingDate = booking.BookingDate,
                        StartTime = booking.StartTime,
                        EndTime = booking.EndTime,
                        NumberOfPeople = booking.NumberOfPeople,
                        TotalAmount = booking.TotalAmount,
                        DepositAmount = booking.DepositAmount,
                        RemainingAmount = booking.RemainingAmount,
                        Status = booking.Status,
                        Location = workingSpace.Location 
                    };

                    bookingResponseList.Add(bookingResponse);
                }

                return bookingResponseList;
            }
            catch (Exception ex)
            {
                // Log the exception and rethrow it
                throw new Exception("An error occurred while retrieving completed bookings: " + ex.Message);
            }
        }
    }
}
