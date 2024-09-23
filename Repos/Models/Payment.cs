namespace Quik_BookingApp.Models
{
    public class Payment
    {
        public string PaymentId { get; set; }
        public string BookingId { get; set; } // Foreign key for Booking
        public decimal Amount { get; set; }
        public string PaymentMethod { get; set; } // E.g., Credit Card, PayPal, etc.
        public DateTime PaymentDate { get; set; }

        public Booking Booking { get; set; }

    }

}
