﻿using System;
using System.Configuration;
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

            // Định nghĩa các mối quan hệ giữa các thực thể
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

            modelBuilder.Entity<WorkingSpace>()
                .HasMany(ws => ws.Images)
                .WithOne(iws => iws.WorkingSpace)
                .HasForeignKey(iws => iws.SpaceId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<WorkingSpace>()
                .HasMany(ws => ws.Amenities) // Define the relationship with Amenity
                .WithOne()
                .HasForeignKey(a => a.SpaceId)
                .OnDelete(DeleteBehavior.Cascade); // Cascade delete for amenities

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

            // Seed dữ liệu (nếu cần)
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

            modelBuilder.Entity<WorkingSpace>().HasData(
                new WorkingSpace
                {
                    SpaceId = "space001",
                    ImageId = "img_space001",
                    BusinessId = "business001",
                    Title = "Cozy Private Office",
                    Description = "A private office space for up to 4 people.",
                    PricePerHour = 25.00M,
                    Capacity = 4,
                    Location = "123 Main Street, Room 101"
                }
            );

            modelBuilder.Entity<Amenity>().HasData(
                new Amenity
                {
                    SpaceId = "space001",
                    AmenityId = "facility1",
                    AmenityText = "Air conditioner free"
                },
                new Amenity
                {
                    SpaceId = "space001",
                    AmenityId = "facility2",
                    AmenityText = "Wifi's room free"
                });

            // Seed dữ liệu cho ImageWS
            modelBuilder.Entity<ImageWS>().HasData(
                new ImageWS
                {
                    ImageId = "img_space001", // Đảm bảo ImageId là chuỗi
                    SpaceId = "space001",
                    WorkingSpaceName = "Cozy Private Office",
                    ImageUrl = "https://example.com/images/space001_image1.jpg",
                    WSCode = "WS001",
                    WSImages = null
                }

            );

            modelBuilder.Entity<Booking>().HasData(
                new Booking
                {
                    BookingId = "booking001",
                    Username = "john_doe",
                    SpaceId = "space001",
                    PaymentId = "payment001",
                    BookingDate = DateTime.Now.Date,
                    StartTime = DateTime.Now.AddHours(1),
                    EndTime = DateTime.Now.AddHours(3),
                    NumberOfPeople = 4,
                    TotalAmount = 200000,
                    DepositAmount = 20000,
                    RemainingAmount = 180000,
                    Status = "Hoàn tất"
                }
            );

            modelBuilder.Entity<Payment>().HasData(
                new Payment
                {
                    PaymentId = "payment001",
                    BookingId = "booking001",
                    Amount = 50.00M,
                    PaymentMethod = "Credit Card",
                    PaymentDate = DateTime.Now,
                    PaymentStatus = "Success",
                    VNPayTransactionId = "VNPay12345",
                    VNPayResponseCode = "00",
                    PaymentUrl = "https://example.com/payment/payment001"
                }
            );
        }
    }
}
