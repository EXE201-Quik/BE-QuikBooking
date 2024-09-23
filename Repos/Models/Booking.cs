﻿namespace Quik_BookingApp.Models
{
    public class Booking
    {
        public string BookingId { get; set; }
        public string UserId { get; set; } // Foreign key for User
        public string SpaceId { get; set; } // Foreign key for WorkingSpace
        public DateTime BookingDate { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public decimal TotalAmount { get; set; }
        public string Status { get; set; }

        public User User { get; set; }
        public WorkingSpace WorkingSpace { get; set; }

        // Add the following property to fix the error
        public ICollection<Payment> Payments { get; set; }
    }


}
