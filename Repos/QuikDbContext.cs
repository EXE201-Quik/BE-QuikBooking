using System;
using Microsoft.EntityFrameworkCore;
using Quik_BookingApp.Models;
using Quik_BookingApp.Repos.Models;

namespace Quik_BookingApp.Repos
{
    public class QuikDbContext : DbContext
    {
        public QuikDbContext() { }
        public QuikDbContext(DbContextOptions<QuikDbContext> options) : base(options) { }

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

            // Define primary keys
            modelBuilder.Entity<User>().HasKey(u => u.Username);
            modelBuilder.Entity<Business>().HasKey(b => b.BusinessId);
            modelBuilder.Entity<WorkingSpace>().HasKey(ws => ws.SpaceId);
            modelBuilder.Entity<Booking>().HasKey(b => b.BookingId);
            modelBuilder.Entity<Payment>().HasKey(p => p.PaymentId);
            modelBuilder.Entity<ImageWS>().HasKey(iws => iws.ImageId);
            modelBuilder.Entity<Tempuser>().HasKey(tu => tu.Id);
            modelBuilder.Entity<OtpManager>().HasKey(om => om.Id);
            modelBuilder.Entity<PwdManager>().HasKey(pm => pm.Id);
            modelBuilder.Entity<TblRefreshToken>().HasKey(rt => new { rt.UserId, rt.TokenId });

            // Relationships
            modelBuilder.Entity<Business>()
                .HasOne(b => b.Owner)
                .WithMany(u => u.Businesses)
                .HasForeignKey(b => b.OwnerId)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<WorkingSpace>()
                .HasOne(ws => ws.Business)
                .WithMany(b => b.WorkingSpaces)
                .HasForeignKey(ws => ws.BusinessId)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<Booking>()
                .HasOne(b => b.User)
                .WithMany(u => u.Bookings)
                .HasForeignKey(b => b.UserId)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<Booking>()
                .HasOne(b => b.WorkingSpace)
                .WithMany(ws => ws.Bookings)
                .HasForeignKey(b => b.SpaceId)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<Payment>()
                .HasOne(p => p.Booking)
                .WithMany(b => b.Payments)
                .HasForeignKey(p => p.BookingId)
                .OnDelete(DeleteBehavior.NoAction);

            // Seed User Data
            modelBuilder.Entity<User>().HasData(
                new User
                {
                    Username = "john_doe",
                    Name = "John Doe",
                    Email = "john@example.com",
                    Password = "hashedpassword123",
                    Role = "User",
                    ImageId = "img001",
                    PhoneNumber = "1234567890",
                    OTPVerified = true,
                    IsActive = true,
                    IsLocked = false,
                    Status = "Active"
                },
                new User
                {
                    Username = "jane_business",
                    Name = "Jane Business",
                    Email = "jane@example.com",
                    Password = "hashedpassword456",
                    Role = "Business",
                    ImageId = "img002",
                    PhoneNumber = "0987654321",
                    OTPVerified = true,
                    IsActive = true,
                    IsLocked = false,
                    Status = "Active"
                }
            );

            // Seed Business Data
            modelBuilder.Entity<Business>().HasData(
                new Business
                {
                    BusinessId = "business001",
                    BusinessName = "Jane's Workspace",
                    OwnerId = "jane_business",
                    Location = "123 Main Street",
                    Description = "A cozy working space for startups.",
                    Rating = 4.5
                }
            );

            // Seed WorkingSpace Data
            modelBuilder.Entity<WorkingSpace>().HasData(
                new WorkingSpace
                {
                    SpaceId = "space001",
                    BusinessId = "business001",
                    ImageId = "img_space001",
                    Title = "Cozy Private Office",
                    Description = "A private office space for up to 4 people.",
                    PricePerHour = 25.00M,
                    Capacity = 4,
                    Location = "123 Main Street, Room 101"
                }
            );

            // Seed Booking Data
            modelBuilder.Entity<Booking>().HasData(
                new Booking
                {
                    BookingId = "booking001",
                    UserId = "john_doe",
                    SpaceId = "space001",
                    BookingDate = DateTime.Now.Date,
                    StartTime = DateTime.Now.AddHours(1),
                    EndTime = DateTime.Now.AddHours(3),
                    TotalAmount = 50.00M,
                    Status = "Confirmed"
                }
            );

            // Seed Payment Data
            modelBuilder.Entity<Payment>().HasData(
                new Payment
                {
                    PaymentId = "payment001",
                    BookingId = "booking001",
                    Amount = 50.00M,
                    PaymentMethod = "Credit Card",
                    PaymentDate = DateTime.Now
                }
            );
        }
    }
}
