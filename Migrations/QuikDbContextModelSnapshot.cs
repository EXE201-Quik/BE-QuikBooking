﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Quik_BookingApp.Repos;

#nullable disable

namespace QuikBookingApp.Migrations
{
    [DbContext(typeof(QuikDbContext))]
    partial class QuikDbContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "7.0.0")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder);

            modelBuilder.Entity("Quik_BookingApp.Models.Booking", b =>
                {
                    b.Property<string>("BookingId")
                        .HasColumnType("nvarchar(450)");

                    b.Property<DateTime>("BookingDate")
                        .HasColumnType("datetime2");

                    b.Property<DateTime>("EndTime")
                        .HasColumnType("datetime2");

                    b.Property<string>("SpaceId")
                        .IsRequired()
                        .HasColumnType("nvarchar(450)");

                    b.Property<DateTime>("StartTime")
                        .HasColumnType("datetime2");

                    b.Property<string>("Status")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<decimal>("TotalAmount")
                        .HasColumnType("decimal(18,2)");

                    b.Property<string>("Username")
                        .IsRequired()
                        .HasColumnType("nvarchar(450)");

                    b.HasKey("BookingId");

                    b.HasIndex("SpaceId");

                    b.HasIndex("Username");

                    b.ToTable("Bookings");

                    b.HasData(
                        new
                        {
                            BookingId = "booking001",
                            BookingDate = new DateTime(2024, 9, 25, 0, 0, 0, 0, DateTimeKind.Local),
                            EndTime = new DateTime(2024, 9, 25, 18, 36, 18, 250, DateTimeKind.Local).AddTicks(6958),
                            SpaceId = "space001",
                            StartTime = new DateTime(2024, 9, 25, 16, 36, 18, 250, DateTimeKind.Local).AddTicks(6953),
                            Status = "Confirmed",
                            TotalAmount = 50.00m,
                            Username = "john_doe"
                        });
                });

            modelBuilder.Entity("Quik_BookingApp.Models.Business", b =>
                {
                    b.Property<string>("BusinessId")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("BusinessName")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Description")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Location")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("OwnerId")
                        .IsRequired()
                        .HasColumnType("nvarchar(450)");

                    b.Property<double>("Rating")
                        .HasColumnType("float");

                    b.HasKey("BusinessId");

                    b.HasIndex("OwnerId");

                    b.ToTable("Businesses");

                    b.HasData(
                        new
                        {
                            BusinessId = "business001",
                            BusinessName = "Jane's Workspace",
                            Description = "A cozy working space for startups.",
                            Location = "123 Main Street",
                            OwnerId = "jane_business",
                            Rating = 4.5
                        });
                });

            modelBuilder.Entity("Quik_BookingApp.Models.Payment", b =>
                {
                    b.Property<string>("PaymentId")
                        .HasColumnType("nvarchar(450)");

                    b.Property<decimal>("Amount")
                        .HasColumnType("decimal(18,2)");

                    b.Property<string>("BookingId")
                        .IsRequired()
                        .HasColumnType("nvarchar(450)");

                    b.Property<DateTime>("PaymentDate")
                        .HasColumnType("datetime2");

                    b.Property<string>("PaymentMethod")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("PaymentId");

                    b.HasIndex("BookingId");

                    b.ToTable("Payments");

                    b.HasData(
                        new
                        {
                            PaymentId = "payment001",
                            Amount = 50.00m,
                            BookingId = "booking001",
                            PaymentDate = new DateTime(2024, 9, 25, 15, 36, 18, 250, DateTimeKind.Local).AddTicks(6970),
                            PaymentMethod = "Credit Card"
                        });
                });

            modelBuilder.Entity("Quik_BookingApp.Models.User", b =>
                {
                    b.Property<string>("Username")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("Email")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("ImageId")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<bool>("IsActive")
                        .HasColumnType("bit");

                    b.Property<bool>("IsLocked")
                        .HasColumnType("bit");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<bool>("OTPVerified")
                        .HasColumnType("bit");

                    b.Property<string>("Password")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("PhoneNumber")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Role")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Status")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Username");

                    b.ToTable("Users");

                    b.HasData(
                        new
                        {
                            Username = "john_doe",
                            Email = "john@example.com",
                            ImageId = "img001",
                            IsActive = true,
                            IsLocked = false,
                            Name = "John Doe",
                            OTPVerified = true,
                            Password = "hashedpassword123",
                            PhoneNumber = "1234567890",
                            Role = "User",
                            Status = "Active"
                        },
                        new
                        {
                            Username = "jane_business",
                            Email = "jane@example.com",
                            ImageId = "img002",
                            IsActive = true,
                            IsLocked = false,
                            Name = "Jane Business",
                            OTPVerified = true,
                            Password = "hashedpassword456",
                            PhoneNumber = "0987654321",
                            Role = "Business",
                            Status = "Active"
                        });
                });

            modelBuilder.Entity("Quik_BookingApp.Models.WorkingSpace", b =>
                {
                    b.Property<string>("SpaceId")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("BusinessId")
                        .IsRequired()
                        .HasColumnType("nvarchar(450)");

                    b.Property<int>("Capacity")
                        .HasColumnType("int");

                    b.Property<string>("Description")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("ImageId")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Location")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<decimal>("PricePerHour")
                        .HasColumnType("decimal(18,2)");

                    b.Property<string>("Title")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("SpaceId");

                    b.HasIndex("BusinessId");

                    b.ToTable("WorkingSpaces");

                    b.HasData(
                        new
                        {
                            SpaceId = "space001",
                            BusinessId = "business001",
                            Capacity = 4,
                            Description = "A private office space for up to 4 people.",
                            ImageId = "img_space001",
                            Location = "123 Main Street, Room 101",
                            PricePerHour = 25.00m,
                            Title = "Cozy Private Office"
                        });
                });

            modelBuilder.Entity("Quik_BookingApp.Repos.Models.ImageWS", b =>
                {
                    b.Property<int>("ImageId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("ImageId"));

                    b.Property<string>("WSCode")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<byte[]>("WSImages")
                        .IsRequired()
                        .HasColumnType("varbinary(max)");

                    b.Property<string>("WorkingSpaceName")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("ImageId");

                    b.ToTable("Images");
                });

            modelBuilder.Entity("Quik_BookingApp.Repos.Models.OtpManager", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<DateTime>("CreatedDate")
                        .HasColumnType("datetime2");

                    b.Property<DateTime>("Expiration")
                        .HasColumnType("datetime2");

                    b.Property<string>("OtpText")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("OtpType")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Username")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.ToTable("OtpManagers");
                });

            modelBuilder.Entity("Quik_BookingApp.Repos.Models.PwdManager", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<DateTime>("ModifyDate")
                        .HasColumnType("datetime2");

                    b.Property<string>("Password")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Username")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.ToTable("PwdManagers");
                });

            modelBuilder.Entity("Quik_BookingApp.Repos.Models.TblRefreshToken", b =>
                {
                    b.Property<string>("UserId")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("TokenId")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("RefreshToken")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("UserId", "TokenId");

                    b.ToTable("TblRefreshtokens");
                });

            modelBuilder.Entity("Quik_BookingApp.Repos.Models.Tempuser", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("Code")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Email")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Password")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Phone")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.ToTable("Tempusers");
                });

            modelBuilder.Entity("Quik_BookingApp.Models.Booking", b =>
                {
                    b.HasOne("Quik_BookingApp.Models.WorkingSpace", "WorkingSpace")
                        .WithMany("Bookings")
                        .HasForeignKey("SpaceId")
                        .OnDelete(DeleteBehavior.NoAction)
                        .IsRequired();

                    b.HasOne("Quik_BookingApp.Models.User", "User")
                        .WithMany("Bookings")
                        .HasForeignKey("Username")
                        .OnDelete(DeleteBehavior.NoAction)
                        .IsRequired();

                    b.Navigation("User");

                    b.Navigation("WorkingSpace");
                });

            modelBuilder.Entity("Quik_BookingApp.Models.Business", b =>
                {
                    b.HasOne("Quik_BookingApp.Models.User", "Owner")
                        .WithMany("Businesses")
                        .HasForeignKey("OwnerId")
                        .OnDelete(DeleteBehavior.NoAction)
                        .IsRequired();

                    b.Navigation("Owner");
                });

            modelBuilder.Entity("Quik_BookingApp.Models.Payment", b =>
                {
                    b.HasOne("Quik_BookingApp.Models.Booking", "Booking")
                        .WithMany("Payments")
                        .HasForeignKey("BookingId")
                        .OnDelete(DeleteBehavior.NoAction)
                        .IsRequired();

                    b.Navigation("Booking");
                });

            modelBuilder.Entity("Quik_BookingApp.Models.WorkingSpace", b =>
                {
                    b.HasOne("Quik_BookingApp.Models.Business", "Business")
                        .WithMany("WorkingSpaces")
                        .HasForeignKey("BusinessId")
                        .OnDelete(DeleteBehavior.NoAction)
                        .IsRequired();

                    b.Navigation("Business");
                });

            modelBuilder.Entity("Quik_BookingApp.Models.Booking", b =>
                {
                    b.Navigation("Payments");
                });

            modelBuilder.Entity("Quik_BookingApp.Models.Business", b =>
                {
                    b.Navigation("WorkingSpaces");
                });

            modelBuilder.Entity("Quik_BookingApp.Models.User", b =>
                {
                    b.Navigation("Bookings");

                    b.Navigation("Businesses");
                });

            modelBuilder.Entity("Quik_BookingApp.Models.WorkingSpace", b =>
                {
                    b.Navigation("Bookings");
                });
#pragma warning restore 612, 618
        }
    }
}
