namespace Quik_BookingApp.BOs.Request
{
    public class VNPayPaymentRequestModel
    {
        public string OrderId { get; set; }
        public decimal Amount { get; set; }
        public string OrderDescription { get; set; }
        public string BankCode { get; set; }
        public string Language { get; set; }
    }

}
