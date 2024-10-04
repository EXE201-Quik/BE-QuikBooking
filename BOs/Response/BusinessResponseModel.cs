namespace Quik_BookingApp.BOs.Response
{
    public class BusinessResponseModel
    {
        public string BusinessId { get; set; }
        public string BusinessName { get; set; }
        public string OwnerId { get; set; } // Foreign key for User
        public string Location { get; set; }
        public string Description { get; set; }
        public double Rating { get; set; }
    }
}
