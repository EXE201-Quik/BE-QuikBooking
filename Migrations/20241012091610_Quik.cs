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
                name: "Businesses",
                columns: table => new
                {
                    BusinessId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    BusinessName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    OwnerId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Location = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Businesses", x => x.BusinessId);
                    table.ForeignKey(
                        name: "FK_Businesses_Users_OwnerId",
                        column: x => x.OwnerId,
                        principalTable: "Users",
                        principalColumn: "Username");
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
                table: "Users",
                columns: new[] { "Username", "Email", "ImageId", "IsActive", "IsLocked", "Name", "OTPVerified", "Password", "PhoneNumber", "Role", "Status" },
                values: new object[,]
                {
                    { "jane_business", "jane@example.com", "img002", true, false, "Jane Business", true, "hashedpassword456", "0987654321", "Business", "Active" },
                    { "john_doe", "john@example.com", "img001", true, false, "John Doe", true, "hashedpassword123", "1234567890", "User", "Active" }
                });

            migrationBuilder.InsertData(
                table: "Businesses",
                columns: new[] { "BusinessId", "BusinessName", "Description", "Location", "OwnerId" },
                values: new object[] { "business001", "Jane's Workspace", "A cozy working space for startups.", "123 Main Street", "jane_business" });

            migrationBuilder.InsertData(
                table: "WorkingSpaces",
                columns: new[] { "SpaceId", "BusinessId", "Capacity", "Description", "ImageId", "Location", "PricePerHour", "RoomType", "Title" },
                values: new object[,]
                {
                    { "space001", "business001", 4, "A private office space for up to 4 people.", "img_space001", "123 Main Street, Room 101", 25000m, "Không gian văn phòng", "Cozy Private Office" },
                    { "space002", "business001", 10, "An open workspace for freelancers and small teams.", "img_space002", "123 Main Street, Room 102", 15000m, "Không gian làm việc chung", "Modern Shared Workspace" },
                    { "space003", "business001", 20, "A spacious conference room equipped with A/V facilities.", "img_space003", "123 Main Street, Room 201", 50000m, "Phòng họp", "Conference Room A" },
                    { "space004", "business001", 8, "A quiet study hub with individual workstations.", "img_space004", "123 Main Street, Room 103", 10000m, "Study hub", "Study Hub" },
                    { "space005", "business001", 2, "A premium office space with stunning views.", "img_space005", "123 Main Street, Room 104", 75000m, "Không gian văn phòng", "Executive Office" }
                });

            migrationBuilder.InsertData(
                table: "Amenities",
                columns: new[] { "AmenityId", "AmenityText", "SpaceId" },
                values: new object[,]
                {
                    { "facility1", "Air conditioner free", "space001" },
                    { "facility2", "Wifi's room free", "space001" }
                });

            migrationBuilder.InsertData(
                table: "Bookings",
                columns: new[] { "BookingId", "BookingDate", "DepositAmount", "EndTime", "NumberOfPeople", "PaymentId", "RemainingAmount", "SpaceId", "StartTime", "Status", "TotalAmount", "Username" },
                values: new object[] { "booking001", new DateTime(2024, 10, 12, 0, 0, 0, 0, DateTimeKind.Local), 20000m, new DateTime(2024, 10, 12, 19, 16, 10, 662, DateTimeKind.Local).AddTicks(6279), 4, new Guid("02d56bd5-35fa-4edd-95b1-ba8b2ea77c8d"), 180000m, "space001", new DateTime(2024, 10, 12, 17, 16, 10, 662, DateTimeKind.Local).AddTicks(6272), "Hoàn tất", 200000m, "john_doe" });

            migrationBuilder.InsertData(
                table: "Images",
                columns: new[] { "ImageId", "ImageUrl", "SpaceId", "WSCode", "WSImages", "WorkingSpaceName" },
                values: new object[] { "img_space001", "https://example.com/images/space001_image1.jpg", "space001", "WS001", null, "Cozy Private Office" });

            migrationBuilder.InsertData(
                table: "Payments",
                columns: new[] { "PaymentId", "Amount", "BookingId", "PaymentDate", "PaymentMethod", "PaymentStatus", "PaymentUrl", "VNPayResponseCode", "VNPayTransactionId" },
                values: new object[] { new Guid("89f12688-27fe-4863-b336-198b97929780"), 50000.0, "booking001", new DateTime(2024, 10, 12, 16, 16, 10, 662, DateTimeKind.Local).AddTicks(6297), "Credit Card", "Success", "toexample@gmail.com", "OK", "VNPay001" });

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
                name: "IX_Businesses_OwnerId",
                table: "Businesses",
                column: "OwnerId");

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
                name: "TblRefreshtokens");

            migrationBuilder.DropTable(
                name: "Tempusers");

            migrationBuilder.DropTable(
                name: "Bookings");

            migrationBuilder.DropTable(
                name: "WorkingSpaces");

            migrationBuilder.DropTable(
                name: "Businesses");

            migrationBuilder.DropTable(
                name: "Users");
        }
    }
}
