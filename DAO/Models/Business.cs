namespace Quik_BookingApp.DAO.Models

{
    public class Business
    {
        public string BusinessId { get; set; }
        public string BusinessName { get; set; }
        public string OwnerId { get; set; } // Foreign key for User
        public string Location { get; set; }
        public string Description { get; set; }
        public double Rating { get; set; }

        public User Owner { get; set; }
        public ICollection<WorkingSpace> WorkingSpaces { get; set; }
    }
}
