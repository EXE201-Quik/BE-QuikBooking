namespace Quik_BookingApp.BOs.Request
{
    public class VNPayPaymentRequestModel
    {
        public string OrderType { get; set; }
        public double Amount { get; set; }
        public string OrderDescription { get; set; }
        public string Name { get; set; }
        public string UserId { get; set; }
    }

}
