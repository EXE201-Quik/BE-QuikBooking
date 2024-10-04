
namespace Quik_BookingApp.DAO.Models
{
    public class WorkingSpace
    {
        public string SpaceId { get; set; }
        public string BusinessId { get; set; } // Foreign key for Business
        public string ImageId { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public decimal PricePerHour { get; set; }
        public int Capacity { get; set; }
        public string Location { get; set; }

        public Business Business { get; set; }
        public ICollection<Booking> Bookings { get; set; }
    }

}
