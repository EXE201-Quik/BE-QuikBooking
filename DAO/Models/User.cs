﻿namespace Quik_BookingApp.DAO.Models
{
    public class User
    {
        public string Username { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string Role { get; set; } // "User", "Business", "Admin"
        public string ImageId { get; set; }
        public string PhoneNumber { get; set; }
        public bool OTPVerified { get; set; }
        public bool IsActive { get; set; }
        public bool IsLocked { get; set; }
        public string Status { get; set; }

        public ICollection<Booking> Bookings { get; set; }
        public ICollection<Business> Businesses { get; set; }
    }


}
