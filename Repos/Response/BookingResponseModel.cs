namespace Quik_BookingApp.Repos.Response
{
    public class BookingResponseModel
    {
        public string BookingId { get; set; }
        public string Username { get; set; } // Foreign key for User
        public string SpaceId { get; set; } // Foreign key for WorkingSpace
        public DateTime BookingDate { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public decimal TotalAmount { get; set; }
        public string Status { get; set; }
    }
}
