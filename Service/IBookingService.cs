using Quik_BookingApp.Helper;
using Quik_BookingApp.Models;
using Quik_BookingApp.Repos.Request;
using Quik_BookingApp.Repos.Response;

namespace Quik_BookingApp.Service
{
    public interface IBookingService
    {
        Task<List<BookingResponseModel>> GetAllBookings();
        Task<APIResponse> BookSpace(BookingRequestModel bookingRequest);
        Task<APIResponse> GetBookingById(string bookingId);
        Task<APIResponse> UpdateBooking(string bookingId, Booking bookingRequest);
        Task<APIResponse> DeleteBooking(string bookingId);
    }
}
