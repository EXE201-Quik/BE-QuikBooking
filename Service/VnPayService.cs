﻿using Microsoft.EntityFrameworkCore;
using Quik_BookingApp.BOs.Request;
using Quik_BookingApp.BOs.Response;
using Quik_BookingApp.DAO;
using Quik_BookingApp.Helper;
using Quik_BookingApp.Repos.Interface;

namespace Quik_BookingApp.Service
{
    public class VNPayService : IVnPayService
    {
        // Thông tin từ VNPay như URL, MerchantID, và secret key
        private readonly string vnp_Url = "https://sandbox.vnpayment.vn/paymentv2/vpcpay.html";
        private readonly string vnp_TmnCode = "4G4LHBIP"; 
        private readonly string vnp_HashSecret = "N86NFRJGSI8UR67Q3LA7GMB9HM8KSX3T"; 

        // Tạo URL thanh toán dựa trên thông tin thanh toán
        public string CreatePaymentUrl(double amount, string bookingId, string name)
        {
            var pay = new VnPayLibrary();

            // Thêm các tham số yêu cầu từ VNPay
            pay.AddRequestData("vnp_Version", "2.1.0");
            pay.AddRequestData("vnp_Command", "pay");
            pay.AddRequestData("vnp_TmnCode", vnp_TmnCode);
            pay.AddRequestData("vnp_Amount", ((long)amount * 100).ToString()); // Số tiền phải nhân với 100
            pay.AddRequestData("vnp_TxnRef", bookingId.ToString());
            pay.AddRequestData("vnp_OrderInfo", "Thanh toan cho booking #" + bookingId);
            pay.AddRequestData("vnp_Locale", "vn");
            pay.AddRequestData("vnp_ReturnUrl", "YourReturnUrl"); // Địa chỉ trả về sau khi thanh toán
            pay.AddRequestData("vnp_IpAddr", "YourIpAddress"); // IP của người dùng
            pay.AddRequestData("vnp_CreateDate", DateTime.Now.ToString("yyyyMMddHHmmss"));

            // Tạo URL
            var paymentUrl = pay.CreateRequestUrl(vnp_Url, vnp_HashSecret);
            return paymentUrl;
        }

        // Xác thực chữ ký bảo mật từ VNPay
        public bool ValidateSignature(VNPayCallbackModel model)
        {
            var vnpayData = new VnPayLibrary();

            // Thêm dữ liệu từ callback để kiểm tra chữ ký
            vnpayData.AddResponseData("vnp_TxnRef", model.vnp_TxnRef);
            vnpayData.AddResponseData("vnp_ResponseCode", model.vnp_ResponseCode);
            vnpayData.AddResponseData("vnp_SecureHash", model.vnp_SecureHash);

            // Xác thực chữ ký từ VNPay
            return vnpayData.ValidateSignature(model.vnp_SecureHash, vnp_HashSecret);
        }
    }



    //public string CreatePaymentUrl(VNPayPaymentRequestModel model, HttpContext context)
    //{
    //    var timeZoneById = TimeZoneInfo.FindSystemTimeZoneById(_configuration["TimeZoneId"]);
    //    var timeNow = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, timeZoneById);
    //    var tick = DateTime.Now.Ticks.ToString();
    //    var pay = new VnPayLibrary();
    //    var urlCallBack = _configuration["PaymentCallBack:ReturnUrl"];

    //    pay.AddRequestData("vnp_Version", _configuration["Vnpay:Version"]);
    //    pay.AddRequestData("vnp_Command", _configuration["Vnpay:Command"]);
    //    pay.AddRequestData("vnp_TmnCode", _configuration["Vnpay:TmnCode"]);
    //    pay.AddRequestData("vnp_Amount", ((int)model.Amount * 100).ToString()); // Amount converted to VNPay format
    //    pay.AddRequestData("vnp_CreateDate", timeNow.ToString("yyyyMMddHHmmss"));
    //    pay.AddRequestData("vnp_CurrCode", _configuration["Vnpay:CurrCode"]);
    //    pay.AddRequestData("vnp_IpAddr", pay.GetIpAddress(context));
    //    pay.AddRequestData("vnp_Locale", _configuration["Vnpay:Locale"]);
    //    pay.AddRequestData("vnp_OrderInfo", $"{model.Name} - Booking {model.BookingId}"); // Order information with name and booking ID
    //    pay.AddRequestData("vnp_OrderType", "billpayment"); // Assuming a fixed type for now
    //    pay.AddRequestData("vnp_ReturnUrl", urlCallBack);
    //    pay.AddRequestData("vnp_TxnRef", tick); // Unique transaction reference

    //    var paymentUrl = pay.CreateRequestUrl(_configuration["Vnpay:BaseUrl"], _configuration["Vnpay:HashSecret"]);

    //    // Save the generated payment URL to the Payment record in the database
    //    var payment = _context.Payments.FirstOrDefault(p => p.BookingId == model.BookingId);
    //    if (payment != null)
    //    {
    //        payment.PaymentUrl = paymentUrl; // Update the Payment record with the generated URL
    //        _context.SaveChanges(); // Save the updated record
    //    }

    //    return paymentUrl; // Return the generated payment URL
    //}

    //public VNPayPaymentResponseModel PaymentExecute(IQueryCollection collections)
    //{
    //    var pay = new VnPayLibrary();
    //    var response = pay.GetFullResponseData(collections, _configuration["Vnpay:HashSecret"]);

    //    return response;
    //}
}
