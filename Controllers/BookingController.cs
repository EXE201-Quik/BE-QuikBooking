using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Quik_BookingApp.Container;
using Quik_BookingApp.Models;
using Quik_BookingApp.Repos;
using Quik_BookingApp.Repos.Request;
using Quik_BookingApp.Service;

namespace Quik_BookingApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BookingController : ControllerBase
    {
        private readonly IBookingService _service;
        private readonly QuikDbContext _dbContext;

        public BookingController(IBookingService service, QuikDbContext _dbContext)
        {
            this._service = service;
            this._dbContext = _dbContext;
        }

        //GET: api/Booking
        [HttpGet("GetAllBookings")]
        public async Task<IActionResult> GetAllBookings()
        {
            var data = await _service.GetAllBookings();
            if (data == null || data.Count == 0)
            {
                return NotFound();
            }
            return Ok(data);
        }

        //POST: api/CreateBooking
        [HttpPost("CreateBooking")]
        public async Task<IActionResult> CreateBooking([FromBody] BookingRequestModel bookingDto)
        {
            if (bookingDto == null)
            {
                return BadRequest("Booking data is required.");
            }

            try
            {
                var createdBooking = await _service.BookSpace(bookingDto);
                return CreatedAtAction(nameof(GetBookingById), new { id = createdBooking }, createdBooking);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error occurred while creating the booking: {ex.Message}");
            }
        }

        // GET: api/Booking/{id}
        [HttpGet("{id}")]
        public async Task<IActionResult> GetBookingById(string id)
        {
            var booking = await _service.GetBookingById(id);
            if (booking == null)
            {
                return NotFound("Booking not found.");
            }

            return Ok(booking);
        }

        


        // PUT: api/Booking/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateBooking(string id, [FromBody] Booking bookingDto)
        {
            if (id != bookingDto.BookingId)
            {
                return BadRequest("Booking ID mismatch.");
            }

            try
            {
                var updatedBooking = await _service.UpdateBooking(id, bookingDto);
                if (updatedBooking == null)
                {
                    return NotFound("Booking not found.");
                }

                return Ok(updatedBooking);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error occurred while updating the booking: {ex.Message}");
            }
        }

        // DELETE: api/Booking/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteBooking(string id)
        {
            try
            {
                var result = await _service.DeleteBooking(id);

                if (result.ResponseCode == 404) // Check if booking not found
                {
                    return NotFound(result.Message); // Use the message from the response
                }
                else if (result.ResponseCode == 500) // Check for server error
                {
                    return StatusCode(500, result.Message); // Use the message from the response
                }

                return NoContent(); // 204 No Content
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error occurred while deleting the booking: {ex.Message}");
            }
        }

    }
}

