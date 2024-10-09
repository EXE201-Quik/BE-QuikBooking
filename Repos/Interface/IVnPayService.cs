using Quik_BookingApp.BOs.Request;
using Quik_BookingApp.BOs.Response;

namespace Quik_BookingApp.Repos.Interface
{
    public interface IVnPayService
    {
        string CreatePaymentUrl(VNPayPaymentRequestModel model, HttpContext context);
        VNPayPaymentResponseModel PaymentExecute(IQueryCollection collections);
    }
}
