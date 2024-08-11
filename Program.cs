using System.Reflection;
using System.Text.Json.Serialization;
using dotenv.net;
using HealthHub.Source.Config;
using HealthHub.Source.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;

// Load Environment Variables
DotEnv.Load(options: new DotEnvOptions(ignoreExceptions: false));

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.


// Register Jwt Service and Configure it
// builder.Services.AddAuthentication("Bearer").AddJwtBearer();

// Controllers & Views Service
builder.Services.AddControllersWithViews();

// Register the App Configuration Service
builder.Services.AddSingleton<AppConfig>(provider =>
{
    var config = provider.GetRequiredService<IConfiguration>();
    return new AppConfig(config);
});


// Database Service
builder.Services.AddDbContext<HealthHub.Source.Data.AppContext>((serviceProvider, options) =>
{
    var appConfig = serviceProvider.GetRequiredService<AppConfig>();
    var connectionString = appConfig.DatabaseConnection;
    if (string.IsNullOrEmpty(connectionString))
    {
        throw new InvalidOperationException("DB_CONNECTION environment variable is not set.");
    }
    Console.WriteLine($"This is the conn str: {connectionString}");
    options.UseSqlServer(connectionString);
});

// Register User Service
builder.Services.AddTransient<UserService>();

// Register Auth Service
builder.Services.AddTransient<AuthService>();

// Register Email Service
builder.Services.AddTransient<EmailService>();

// Register File Service
builder.Services.AddTransient<FileService>();

// Register Rendering Service
builder.Services.AddTransient<RenderingService>();


builder.Services.AddControllers().AddJsonOptions(options =>
{
    options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
});

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    var xmlFile = "HealthHub.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    options.IncludeXmlComments(xmlPath);
});


//----------------------------------------

var app = builder.Build();

app.MapControllers();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.Run();