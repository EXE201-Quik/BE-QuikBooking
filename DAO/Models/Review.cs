namespace Quik_BookingApp.DAO.Models
{
    public class Review
    {
        public Guid ReviewId { get; set; } // Primary key
        public string Username { get; set; } // Foreign key to User
        public string SpaceId { get; set; } // Foreign key to WorkingSpace
        public int Rating { get; set; } // Rating between 1 to 5
        public string Comment { get; set; } // Review comment
        public DateTime CreatedAt { get; set; } // Date of review creation

        // Navigation properties
        public User User { get; set; }
        public WorkingSpace WorkingSpace { get; set; }
    }
}
