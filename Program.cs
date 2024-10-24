using AutoMapper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Quik_BookingApp.DAO;
using Quik_BookingApp.Extentions;
using Quik_BookingApp.Helper;
using Quik_BookingApp.Modal;
using Quik_BookingApp.Repos.Interface;
using Quik_BookingApp.Service;
using QuikBookingApp.Modal;
using Serilog;
using System;
using System.Configuration;
using System.Text;
using System.Text.Json.Serialization;


var builder = WebApplication.CreateBuilder(args);

//Call firebase initailization
FirebaseInitializer.InitializeFirebase();

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.EnableAnnotations();
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "Quik_BookingApp API", Version = "v1" });

    // Configure Swagger to use JWT Bearer
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "JWT Authorization header using the Bearer scheme.",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            new string[] {}
        }
    });
});

//builder.Services.AddControllers()
//    .AddJsonOptions(options =>
//    {
//        // Enable ReferenceHandler.Preserve for cycle detection
//        options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.Preserve;
//        options.JsonSerializerOptions.WriteIndented = true; // Optional: Pretty print JSON
//    });




builder.Services.AddTransient<EmailService>();
builder.Services.Configure<FirebaseConfiguration>(builder.Configuration.GetSection("Firebase"));
builder.Services.Configure<EmailSettings>(builder.Configuration.GetSection(""));
builder.Services.AddScoped<IFirebaseService, FirebaseService>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddTransient<IBookingService, BookingService>();
builder.Services.AddScoped<IWorkingSpaceService, WorkingSpaceService>();
builder.Services.AddTransient<IBusinessService, BusinessService>();
builder.Services.AddTransient<IRefreshHandler, RefreshHandler>();
builder.Services.AddTransient<IEmailService, EmailService>();
builder.Services.AddTransient<IPaymentService, PaymentService>();
builder.Services.AddTransient<IVnPayService, VNPayService>();
builder.Services.AddScoped<IReviewService, ReviewService>();



builder.Services.AddDbContext<QuikDbContext>(o =>
o.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"), options => options.CommandTimeout(60)));



var _authkey = builder.Configuration.GetValue<string>("JwtSettings:securitykey");
builder.Services.AddAuthentication(item =>
{
    item.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    item.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(item =>
{
    item.RequireHttpsMetadata = true;
    item.SaveToken = true;
    item.TokenValidationParameters = new TokenValidationParameters()
    {
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_authkey)),
        ValidateIssuer = false,
        ValidateAudience = false,
        ClockSkew = TimeSpan.Zero
    };

});

var automapper = new MapperConfiguration(item => item.AddProfile(new AutoMapperHandler()));
IMapper mapper = automapper.CreateMapper();
builder.Services.AddSingleton(mapper);

string logpath = builder.Configuration.GetSection("Logging:Logpath").Value;
var _logger = new LoggerConfiguration()
    .MinimumLevel.Information()
    .MinimumLevel.Override("microsoft", Serilog.Events.LogEventLevel.Warning)
    .Enrich.FromLogContext()
    .WriteTo.File(logpath)
    .CreateLogger();
builder.Logging.AddSerilog(_logger);

var _jwtsetting = builder.Configuration.GetSection("JwtSettings");
builder.Services.Configure<JwtSettings>(_jwtsetting);

var app = builder.Build();

app.UseCors(builder => builder
    .AllowAnyHeader()
    .AllowAnyMethod()
    .AllowCredentials() // to support a SignalR
    .WithOrigins("http://localhost:5173"));

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    await using (var scope = app.Services.CreateAsyncScope())
    {
        var dbContext = scope.ServiceProvider.GetRequiredService<QuikDbContext>();
        //await dbContext.Database.MigrateAsync();
    }

    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

app.UseRouting();
app.UseEndpoints(endpoints =>
{
    endpoints.MapControllers(); 
});

//await app.CreateDbIfNotExists();

app.Run();
