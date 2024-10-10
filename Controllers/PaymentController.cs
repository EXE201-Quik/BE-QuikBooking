using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Quik_BookingApp.BOs.Request;

using Quik_BookingApp.Helper;
using Quik_BookingApp.Repos.Interface;
using Quik_BookingApp.Service;
using Swashbuckle.AspNetCore.Annotations;

namespace Quik_BookingApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PaymentController : ControllerBase
    {
        private readonly VNPayHelper _vnPayHelper;
        private readonly IConfiguration _configuration;
        private readonly IBookingService bookingService;
        private readonly VnPayService _vnPayService;

        public PaymentController(IBookingService bookingService)
        {
            this.bookingService = bookingService;
        }

        [HttpPost]
        public async Task<IActionResult> CreatePaymentUrl([FromBody] VNPayPaymentRequestModel model)
        {
            var paymentUrl = _vnPayService.CreatePaymentUrl(model, HttpContext);
            //var paymentUrl = VnPayService.CreatePaymentUrl(model, HttpContext);
            return Ok(paymentUrl);
        }

        [HttpGet("payment-callback")]
        public async Task<IActionResult> PaymentCallback()
        {
            var response = _vnPayService.PaymentExecute(Request.Query);
            var paymentResponseModel = response;

            // Parse order description để lấy rentalId từ chuỗi trả về
            var parts = paymentResponseModel.OrderDescription?.Split(' ') ?? new string[0];
            Guid rentalId = Guid.Empty;

            if (parts.Length > 1)
            {
                Guid.TryParse(parts[1], out rentalId);
            }

            // Nếu thanh toán thành công
            if (response.Success)
            {
                var paymentRequest = new CreatePaymentRequest
                {
                    PaymentStatus = "Success",
                    Amount = 100000,
                    BookingId = "Booking",
                };
                //await bookingService.paymentService.AddPayment(paymentRequest);

                // Redirect người dùng đến trang thanh toán thành công trên frontend
                return Redirect($"http://localhost:1024/payment-status?status=success&rentalId={rentalId}");
            }
            else
            {
                //var paymentRequest = new CreatePaymentRequest
                //{
                //    PaymentStatus = PaymentStatus.Deleted,
                //    Amount = paymentResponseModel.AmountOfRental,
                //    RentalId = rentalId,
                //};

                //await _serviceManager.paymentService.AddPayment(paymentRequest);

                // Redirect người dùng đến trang thanh toán thất bại trên frontend
                return Redirect($"http://localhost:1024/payment-status?status=failed&rentalId={rentalId}");
            }
        }
    }
}