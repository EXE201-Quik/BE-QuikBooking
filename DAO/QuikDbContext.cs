using System;
using Microsoft.EntityFrameworkCore;
using Quik_BookingApp.DAO.Models;

namespace Quik_BookingApp.DAO
{
    public class QuikDbContext : DbContext
    {
        public QuikDbContext() { }

        public QuikDbContext(DbContextOptions<QuikDbContext> options) : base(options) { }

        // Định nghĩa các DbSet cho các thực thể trong hệ thống
        public DbSet<User> Users { get; set; }
        public DbSet<Business> Businesses { get; set; }
        public DbSet<WorkingSpace> WorkingSpaces { get; set; }
        public DbSet<Booking> Bookings { get; set; }
        public DbSet<Payment> Payments { get; set; }
        public DbSet<TblRefreshToken> TblRefreshtokens { get; set; }
        public DbSet<ImageWS> Images { get; set; }
        public DbSet<Tempuser> Tempusers { get; set; }
        public DbSet<OtpManager> OtpManagers { get; set; }
        public DbSet<PwdManager> PwdManagers { get; set; }
        public DbSet<Amenity> Amenities { get; set; }
        public DbSet<Review> Reviews { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseSqlServer("DefaultConnection");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Định nghĩa khóa chính cho các thực thể
            modelBuilder.Entity<User>().HasKey(u => u.Username);
            modelBuilder.Entity<Business>().HasKey(b => b.BusinessId);
            modelBuilder.Entity<WorkingSpace>().HasKey(ws => ws.SpaceId);
            modelBuilder.Entity<Booking>().HasKey(b => b.BookingId);
            modelBuilder.Entity<Payment>().HasKey(p => p.PaymentId);
            modelBuilder.Entity<ImageWS>().HasKey(iws => iws.ImageId);
            modelBuilder.Entity<Tempuser>().HasKey(tu => tu.Id);
            modelBuilder.Entity<OtpManager>().HasKey(om => om.Id);
            modelBuilder.Entity<PwdManager>().HasKey(pm => pm.Id);
            modelBuilder.Entity<Amenity>().HasKey(a => a.AmenityId);
            modelBuilder.Entity<TblRefreshToken>().HasKey(rt => new { rt.UserId, rt.TokenId });
            modelBuilder.Entity<Review>().HasKey(rt => rt.ReviewId);


            modelBuilder.Entity<WorkingSpace>()
                .HasOne(ws => ws.Business)
                .WithMany(b => b.WorkingSpaces)
                .HasForeignKey(ws => ws.BusinessId)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<WorkingSpace>()
                .HasMany(ws => ws.Images)
                .WithOne(iws => iws.WorkingSpace)
                .HasForeignKey(iws => iws.SpaceId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<WorkingSpace>()
                .HasMany(ws => ws.Amenities)
                .WithOne()
                .HasForeignKey(a => a.SpaceId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Booking>()
                .HasOne(b => b.User)
                .WithMany(u => u.Bookings)
                .HasForeignKey(b => b.Username)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<Booking>()
                .HasOne(b => b.WorkingSpace)
                .WithMany(ws => ws.Bookings)
                .HasForeignKey(b => b.SpaceId)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<Booking>()
                .HasOne(b => b.Payment)
                .WithOne(p => p.Booking)
                .HasForeignKey<Payment>(p => p.BookingId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Review>()
                .HasOne(r => r.User)
                .WithMany(u => u.Reviews) // A user can have many reviews
                .HasForeignKey(r => r.Username)
                .OnDelete(DeleteBehavior.NoAction); // No cascading delete

            modelBuilder.Entity<Review>()
                .HasOne(r => r.WorkingSpace)
                .WithMany(ws => ws.Reviews) // A working space can have many reviews
                .HasForeignKey(r => r.SpaceId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<User>().HasData(
                 new User
                 {
                     Username = "alice_admin",
                     Name = "Alice Admin",
                     Email = "alice.admin@example.com",
                     Password = "hashedpassword789",
                     Role = "Admin",
                     ImageId = "img003",
                     PhoneNumber = "1231231234",
                     OTPVerified = true,
                     IsActive = true,
                     IsLocked = false,
                     Status = "Active"
                 },
                 new User
                 {
                     Username = "bob_member",
                     Name = "Bob Member",
                     Email = "bob.member@example.com",
                     Password = "hashedpassword789",
                     Role = "Member",
                     ImageId = "img004",
                     PhoneNumber = "3213214321",
                     OTPVerified = true,
                     IsActive = true,
                     IsLocked = false,
                     Status = "Active"
                 },
                 new User
                 {
                     Username = "charlie_business",
                     Name = "Charlie Business",
                     Email = "charlie.business@example.com",
                     Password = "hashedpassword789",
                     Role = "Business",
                     ImageId = "img005",
                     PhoneNumber = "6549871230",
                     OTPVerified = true,
                     IsActive = true,
                     IsLocked = false,
                     Status = "Active"
                 },
                 new User
                 {
                     Username = "david_user",
                     Name = "David User",
                     Email = "david.user@example.com",
                     Password = "hashedpassword321",
                     Role = "User",
                     ImageId = "img006",
                     PhoneNumber = "9876543210",
                     OTPVerified = true,
                     IsActive = true,
                     IsLocked = false,
                     Status = "Active"
                 }
             );

            // Seed thêm dữ liệu cho Business
            modelBuilder.Entity<Business>().HasData(
                new Business
                {
                    BusinessId = "business002",
                    BusinessName = "Workspace Deluxe",
                    PhoneNumber = "987654321",
                    Email = "contact@workspace-deluxe.com",
                    Password = "hashedpassword",
                    Location = "456 Elm Street",
                    Description = "A deluxe workspace offering premium services.",
                    Rating = 4
                },
                new Business
                {
                    BusinessId = "business003",
                    BusinessName = "Startup Hub",
                    PhoneNumber = "123456987",
                    Email = "info@startup-hub.com",
                    Password = "hashedpassword123",
                    Location = "789 Startup Blvd",
                    Description = "An energetic space for young startups.",
                    Rating = 5
                    
                },
                new Business
                {
                    BusinessId = "business004",
                    BusinessName = "Freelancers Corner",
                    PhoneNumber = "654321987",
                    Email = "freelancers@corner.com",
                    Password = "hashedpassword789",
                    Location = "101 Freelance Road",
                    Description = "A cozy spot for freelancers.",
                    Rating = 4
                }
            );

            // Seed thêm dữ liệu cho WorkingSpace
            modelBuilder.Entity<WorkingSpace>().HasData(
                new WorkingSpace
                {
                    SpaceId = "space006",
                    ImageId = "img_space006",
                    BusinessId = "business002",
                    Title = "VIP Executive Office",
                    Description = "An executive office with all luxury amenities, including high-speed internet, ergonomic furniture, and personalized services to enhance productivity and comfort. Ideal for high-stakes meetings and presentations.",
                    PricePerHour = 100000,
                    RoomType = "Executive",
                    Capacity = 3,
                    Location = "456 Elm Street, Room 101",
                    Rating = 4.5f
                },
                new WorkingSpace
                {
                    SpaceId = "space007",
                    ImageId = "img_space007",
                    BusinessId = "business003",
                    Title = "Startup Lab",
                    Description = "Perfect for innovative teams, this lab offers a vibrant atmosphere with collaborative spaces, whiteboards, and high-speed internet. It's designed to foster creativity and teamwork, helping your startup thrive.",
                    PricePerHour = 20000,
                    RoomType = "Lab",
                    Capacity = 5,
                    Location = "789 Startup Blvd, Room 303",
                    Rating = 4.5f
                },
                new WorkingSpace
                {
                    SpaceId = "space008",
                    ImageId = "img_space008",
                    BusinessId = "business004",
                    Title = "Freelance Studio",
                    Description = "An open studio designed for remote workers, featuring a relaxed environment with natural light, comfortable seating, and high-speed internet. It's the perfect place for freelancers to get work done efficiently.",
                    PricePerHour = 15000,
                    RoomType = "Studio",
                    Capacity = 6,
                    Location = "101 Freelance Road, Room 102",
                    Rating = 4.3f
                }
            );


            // Seed thêm dữ liệu cho Amenity
            modelBuilder.Entity<Amenity>().HasData(
                new Amenity
                {
                    SpaceId = "space006",
                    AmenityId = "amenity1",
                    AmenityText = "Private bathroom"
                },
                new Amenity
                {
                    SpaceId = "space006",
                    AmenityId = "amenity2",
                    AmenityText = "Free beverages"
                },
                new Amenity
                {
                    SpaceId = "space007",
                    AmenityId = "amenity3",
                    AmenityText = "24/7 access"
                },
                new Amenity
                {
                    SpaceId = "space008",
                    AmenityId = "amenity4",
                    AmenityText = "Free parking"
                }
            );

            // Seed thêm dữ liệu cho Booking
            modelBuilder.Entity<Booking>().HasData(
                new Booking
                {
                    BookingId = "booking002",
                    PaymentId = Guid.NewGuid(),
                    Username = "bob_member",
                    SpaceId = "space006",
                    BookingDate = DateTime.Now.AddDays(-1),
                    StartTime = DateTime.Now.AddHours(2),
                    EndTime = DateTime.Now.AddHours(4),
                    NumberOfPeople = 2,
                    TotalAmount = 200000,
                    DepositAmount = 30000,
                    RemainingAmount = 170000,
                    Status = "Pending"
                },
                new Booking
                {
                    BookingId = "booking003",
                    PaymentId = Guid.NewGuid(),
                    Username = "alice_admin",
                    SpaceId = "space007",
                    BookingDate = DateTime.Now.AddDays(-2),
                    StartTime = DateTime.Now.AddHours(3),
                    EndTime = DateTime.Now.AddHours(5),
                    NumberOfPeople = 3,
                    TotalAmount = 250000,
                    DepositAmount = 40000,
                    RemainingAmount = 210000,
                    Status = "Confirmed"
                }
            );

            // Seed thêm dữ liệu cho Payment
            modelBuilder.Entity<Payment>().HasData(
                new Payment
                {
                    PaymentId = Guid.NewGuid(),
                    BookingId = "booking002",
                    Amount = 30000,
                    PaymentMethod = "PayPal",
                    PaymentDate = DateTime.Now,
                    PaymentStatus = "Pending",
                    VNPayTransactionId = "VNPay002",
                    VNPayResponseCode = "OK",
                    PaymentUrl = "payment002@example.com"
                },
                new Payment
                {
                    PaymentId = Guid.NewGuid(),
                    BookingId = "booking003",
                    Amount = 40000,
                    PaymentMethod = "Credit Card",
                    PaymentDate = DateTime.Now,
                    PaymentStatus = "Success",
                    VNPayTransactionId = "VNPay003",
                    VNPayResponseCode = "OK",
                    PaymentUrl = "payment003@example.com"
                }
            );

        modelBuilder.Entity<Review>().HasData(
                new Review
                {
                    ReviewId = Guid.NewGuid(),
                    Username = "alice_admin",
                    SpaceId = "space006",
                    Rating = 5,
                    Comment = "Amazing experience, highly recommend!",
                    Title = "Fantastic Experience", // New title field
                    CreatedAt = DateTime.Now.AddDays(-5)
                },
                new Review
                {
                    ReviewId = Guid.NewGuid(),
                    Username = "bob_member",
                    SpaceId = "space007",
                    Rating = 4,
                    Comment = "Great place for team collaboration!",
                    Title = "Good Collaboration Space", // New title field
                    CreatedAt = DateTime.Now.AddDays(-3)
                },
                new Review
                {
                    ReviewId = Guid.NewGuid(),
                    Username = "charlie_business",
                    SpaceId = "space008",
                    Rating = 4,
                    Comment = "Nice and quiet workspace.",
                    Title = "Quiet Workspace", // New title field
                    CreatedAt = DateTime.Now.AddDays(-1)
                }
            );

        }
    }
}
