using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace QuikBookingApp.Migrations
{
    /// <inheritdoc />
    public partial class Quik : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Businesses",
                columns: table => new
                {
                    BusinessId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    BusinessName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PhoneNumber = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Email = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Password = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Location = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Businesses", x => x.BusinessId);
                });

            migrationBuilder.CreateTable(
                name: "OtpManagers",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Username = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    OtpText = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    OtpType = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Expiration = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OtpManagers", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "PwdManagers",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Username = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Password = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ModifyDate = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PwdManagers", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "TblRefreshtokens",
                columns: table => new
                {
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    TokenId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    RefreshToken = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TblRefreshtokens", x => new { x.UserId, x.TokenId });
                });

            migrationBuilder.CreateTable(
                name: "Tempusers",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Code = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Email = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Password = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Phone = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Tempusers", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    Username = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Email = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Password = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Role = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ImageId = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PhoneNumber = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    OTPVerified = table.Column<bool>(type: "bit", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    IsLocked = table.Column<bool>(type: "bit", nullable: false),
                    Status = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Username);
                });

            migrationBuilder.CreateTable(
                name: "WorkingSpaces",
                columns: table => new
                {
                    SpaceId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    BusinessId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ImageId = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Title = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PricePerHour = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    RoomType = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Capacity = table.Column<int>(type: "int", nullable: false),
                    Location = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WorkingSpaces", x => x.SpaceId);
                    table.ForeignKey(
                        name: "FK_WorkingSpaces_Businesses_BusinessId",
                        column: x => x.BusinessId,
                        principalTable: "Businesses",
                        principalColumn: "BusinessId");
                });

            migrationBuilder.CreateTable(
                name: "Amenities",
                columns: table => new
                {
                    AmenityId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    SpaceId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    AmenityText = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Amenities", x => x.AmenityId);
                    table.ForeignKey(
                        name: "FK_Amenities_WorkingSpaces_SpaceId",
                        column: x => x.SpaceId,
                        principalTable: "WorkingSpaces",
                        principalColumn: "SpaceId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Bookings",
                columns: table => new
                {
                    BookingId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Username = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    SpaceId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    PaymentId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    BookingDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    StartTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    EndTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    NumberOfPeople = table.Column<int>(type: "int", nullable: false),
                    TotalAmount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    DepositAmount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    RemainingAmount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Status = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Bookings", x => x.BookingId);
                    table.ForeignKey(
                        name: "FK_Bookings_Users_Username",
                        column: x => x.Username,
                        principalTable: "Users",
                        principalColumn: "Username");
                    table.ForeignKey(
                        name: "FK_Bookings_WorkingSpaces_SpaceId",
                        column: x => x.SpaceId,
                        principalTable: "WorkingSpaces",
                        principalColumn: "SpaceId");
                });

            migrationBuilder.CreateTable(
                name: "Images",
                columns: table => new
                {
                    ImageId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    SpaceId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    WorkingSpaceName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ImageUrl = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    WSCode = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    WSImages = table.Column<byte[]>(type: "varbinary(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Images", x => x.ImageId);
                    table.ForeignKey(
                        name: "FK_Images_WorkingSpaces_SpaceId",
                        column: x => x.SpaceId,
                        principalTable: "WorkingSpaces",
                        principalColumn: "SpaceId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Reviews",
                columns: table => new
                {
                    ReviewId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Username = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    SpaceId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Rating = table.Column<float>(type: "real", nullable: false),
                    Comment = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Reviews", x => x.ReviewId);
                    table.ForeignKey(
                        name: "FK_Reviews_Users_Username",
                        column: x => x.Username,
                        principalTable: "Users",
                        principalColumn: "Username");
                    table.ForeignKey(
                        name: "FK_Reviews_WorkingSpaces_SpaceId",
                        column: x => x.SpaceId,
                        principalTable: "WorkingSpaces",
                        principalColumn: "SpaceId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Payments",
                columns: table => new
                {
                    PaymentId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    BookingId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Amount = table.Column<double>(type: "float", nullable: false),
                    PaymentMethod = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PaymentDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    PaymentStatus = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    VNPayTransactionId = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    VNPayResponseCode = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PaymentUrl = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Payments", x => x.PaymentId);
                    table.ForeignKey(
                        name: "FK_Payments_Bookings_BookingId",
                        column: x => x.BookingId,
                        principalTable: "Bookings",
                        principalColumn: "BookingId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "Businesses",
                columns: new[] { "BusinessId", "BusinessName", "Description", "Email", "Location", "Password", "PhoneNumber" },
                values: new object[,]
                {
                    { "business002", "Workspace Deluxe", "A deluxe workspace offering premium services.", "contact@workspace-deluxe.com", "456 Elm Street", "hashedpassword", "987654321" },
                    { "business003", "Startup Hub", "An energetic space for young startups.", "info@startup-hub.com", "789 Startup Blvd", "hashedpassword123", "123456987" },
                    { "business004", "Freelancers Corner", "A cozy spot for freelancers.", "freelancers@corner.com", "101 Freelance Road", "hashedpassword789", "654321987" }
                });

            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "Username", "Email", "ImageId", "IsActive", "IsLocked", "Name", "OTPVerified", "Password", "PhoneNumber", "Role", "Status" },
                values: new object[,]
                {
                    { "alice_admin", "alice.admin@example.com", "img003", true, false, "Alice Admin", true, "hashedpassword789", "1231231234", "Admin", "Active" },
                    { "bob_member", "bob.member@example.com", "img004", true, false, "Bob Member", true, "hashedpassword789", "3213214321", "Member", "Active" },
                    { "charlie_business", "charlie.business@example.com", "img005", true, false, "Charlie Business", true, "hashedpassword789", "6549871230", "Business", "Active" },
                    { "david_user", "david.user@example.com", "img006", true, false, "David User", true, "hashedpassword321", "9876543210", "User", "Active" }
                });

            migrationBuilder.InsertData(
                table: "WorkingSpaces",
                columns: new[] { "SpaceId", "BusinessId", "Capacity", "Description", "ImageId", "Location", "PricePerHour", "RoomType", "Title" },
                values: new object[,]
                {
                    { "space006", "business002", 3, "An executive office with all luxury amenities.", "img_space006", "456 Elm Street, Room 101", 100000m, "Executive", "VIP Executive Office" },
                    { "space007", "business003", 5, "Perfect for small teams working on innovation.", "img_space007", "789 Startup Blvd, Room 303", 20000m, "Lab", "Startup Lab" },
                    { "space008", "business004", 6, "An open studio perfect for remote workers.", "img_space008", "101 Freelance Road, Room 102", 15000m, "Studio", "Freelance Studio" }
                });

            migrationBuilder.InsertData(
                table: "Amenities",
                columns: new[] { "AmenityId", "AmenityText", "SpaceId" },
                values: new object[,]
                {
                    { "amenity1", "Private bathroom", "space006" },
                    { "amenity2", "Free beverages", "space006" },
                    { "amenity3", "24/7 access", "space007" },
                    { "amenity4", "Free parking", "space008" }
                });

            migrationBuilder.InsertData(
                table: "Bookings",
                columns: new[] { "BookingId", "BookingDate", "DepositAmount", "EndTime", "NumberOfPeople", "PaymentId", "RemainingAmount", "SpaceId", "StartTime", "Status", "TotalAmount", "Username" },
                values: new object[,]
                {
                    { "booking002", new DateTime(2024, 10, 16, 16, 58, 1, 680, DateTimeKind.Local).AddTicks(8928), 30000m, new DateTime(2024, 10, 17, 20, 58, 1, 680, DateTimeKind.Local).AddTicks(8950), 2, new Guid("24f99844-dbb8-4ab1-877f-c214b43ec456"), 170000m, "space006", new DateTime(2024, 10, 17, 18, 58, 1, 680, DateTimeKind.Local).AddTicks(8949), "Pending", 200000m, "bob_member" },
                    { "booking003", new DateTime(2024, 10, 15, 16, 58, 1, 680, DateTimeKind.Local).AddTicks(8955), 40000m, new DateTime(2024, 10, 17, 21, 58, 1, 680, DateTimeKind.Local).AddTicks(8957), 3, new Guid("a9b72404-ee69-4f03-b5a5-b23deb983a9a"), 210000m, "space007", new DateTime(2024, 10, 17, 19, 58, 1, 680, DateTimeKind.Local).AddTicks(8956), "Confirmed", 250000m, "alice_admin" }
                });

            migrationBuilder.InsertData(
                table: "Reviews",
                columns: new[] { "ReviewId", "Comment", "CreatedAt", "Rating", "SpaceId", "Username" },
                values: new object[,]
                {
                    { new Guid("43c07812-50ca-4245-8253-9edba6293b18"), "Amazing experience, highly recommend!", new DateTime(2024, 10, 12, 16, 58, 1, 680, DateTimeKind.Local).AddTicks(9049), 5f, "space006", "alice_admin" },
                    { new Guid("f49fcab4-1cb2-49fd-8596-57d7bfb1c06e"), "Nice and quiet workspace.", new DateTime(2024, 10, 16, 16, 58, 1, 680, DateTimeKind.Local).AddTicks(9054), 4f, "space008", "charlie_business" },
                    { new Guid("ff20e165-361f-4f09-b67c-7148c3ab4b3a"), "Great place for team collaboration!", new DateTime(2024, 10, 14, 16, 58, 1, 680, DateTimeKind.Local).AddTicks(9052), 4f, "space007", "bob_member" }
                });

            migrationBuilder.InsertData(
                table: "Payments",
                columns: new[] { "PaymentId", "Amount", "BookingId", "PaymentDate", "PaymentMethod", "PaymentStatus", "PaymentUrl", "VNPayResponseCode", "VNPayTransactionId" },
                values: new object[,]
                {
                    { new Guid("5d003ca5-8753-47c9-a3c9-e969d1457506"), 30000.0, "booking002", new DateTime(2024, 10, 17, 16, 58, 1, 680, DateTimeKind.Local).AddTicks(9028), "PayPal", "Pending", "payment002@example.com", "OK", "VNPay002" },
                    { new Guid("fd45d97e-8a68-45ad-b881-cd9b8cbbd272"), 40000.0, "booking003", new DateTime(2024, 10, 17, 16, 58, 1, 680, DateTimeKind.Local).AddTicks(9031), "Credit Card", "Success", "payment003@example.com", "OK", "VNPay003" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Amenities_SpaceId",
                table: "Amenities",
                column: "SpaceId");

            migrationBuilder.CreateIndex(
                name: "IX_Bookings_SpaceId",
                table: "Bookings",
                column: "SpaceId");

            migrationBuilder.CreateIndex(
                name: "IX_Bookings_Username",
                table: "Bookings",
                column: "Username");

            migrationBuilder.CreateIndex(
                name: "IX_Images_SpaceId",
                table: "Images",
                column: "SpaceId");

            migrationBuilder.CreateIndex(
                name: "IX_Payments_BookingId",
                table: "Payments",
                column: "BookingId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Reviews_SpaceId",
                table: "Reviews",
                column: "SpaceId");

            migrationBuilder.CreateIndex(
                name: "IX_Reviews_Username",
                table: "Reviews",
                column: "Username");

            migrationBuilder.CreateIndex(
                name: "IX_WorkingSpaces_BusinessId",
                table: "WorkingSpaces",
                column: "BusinessId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Amenities");

            migrationBuilder.DropTable(
                name: "Images");

            migrationBuilder.DropTable(
                name: "OtpManagers");

            migrationBuilder.DropTable(
                name: "Payments");

            migrationBuilder.DropTable(
                name: "PwdManagers");

            migrationBuilder.DropTable(
                name: "Reviews");

            migrationBuilder.DropTable(
                name: "TblRefreshtokens");

            migrationBuilder.DropTable(
                name: "Tempusers");

            migrationBuilder.DropTable(
                name: "Bookings");

            migrationBuilder.DropTable(
                name: "Users");

            migrationBuilder.DropTable(
                name: "WorkingSpaces");

            migrationBuilder.DropTable(
                name: "Businesses");
        }
    }
}
