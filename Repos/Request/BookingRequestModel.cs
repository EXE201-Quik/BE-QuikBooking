namespace Quik_BookingApp.Repos.Request
{
    public class BookingRequestModel
    {
        public string Username { get; set; }
        public string SpaceId { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
    }
}
