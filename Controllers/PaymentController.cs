using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Quik_BookingApp.BOs.Request;

using Quik_BookingApp.Helper;

namespace Quik_BookingApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PaymentController : ControllerBase
    {
        private readonly VNPayHelper _vnPayHelper;
        private readonly IConfiguration _configuration;

        public PaymentController(VNPayHelper vnPayHelper, IConfiguration configuration)
        {
            _vnPayHelper = vnPayHelper;
            _configuration = configuration;
        }

        [HttpPost("vnpay")]
        public IActionResult CreateVNPayPayment([FromBody] VNPayPaymentRequestModel request)
        {
            var paymentUrl = _vnPayHelper.CreatePaymentUrl(request, HttpContext);
            return Ok(new { paymentUrl });
        }

        [HttpGet("vnpay_return")]
        public IActionResult VNPayReturn()
        {
            // Xử lý phản hồi từ VNPay
            var vnpayData = Request.Query;
            var vnpay = new VnPayLibrary();

            foreach (var (key, value) in vnpayData)
            {
                if (!string.IsNullOrEmpty(key) && key.StartsWith("vnp_"))
                {
                    vnpay.AddResponseData(key, value);
                }
            }

            var hashSecret = _configuration["VNPaySettings:HashSecret"];
            var isValidSignature = vnpay.ValidateSignature(vnpay.GetResponseData(), hashSecret);

            if (isValidSignature)
            {
                // Xử lý thành công
                return Ok(new { status = "Success", message = "Thanh toán thành công!" });
            }

            return BadRequest(new { status = "Failed", message = "Chữ ký không hợp lệ!" });
        }


    }
}
