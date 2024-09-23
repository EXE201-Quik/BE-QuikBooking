namespace Quik_BookingApp.Modal
{
    public class WorkingSpaceModal
    {
        public int SpaceId { get; set; }
        public int BusinessId { get; set; } // Foreign key for Business
        public string Title { get; set; }
        public string Description { get; set; }
        public decimal PricePerHour { get; set; }
        public int Capacity { get; set; }
        public string Location { get; set; }
        public string ImageUrl { get; set; }
    }
}
