using System.Text.Json.Serialization;
using dotenv.net;
using HealthHub.Source.Services;
using Microsoft.EntityFrameworkCore;

// Load Environment Variables
DotEnv.Load(options: new DotEnvOptions(ignoreExceptions: false));

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddDbContext<AppContext>(options =>
{
    var connectionString = Environment.GetEnvironmentVariable("DB_CONNECTION");
    if (string.IsNullOrEmpty(connectionString))
    {
        throw new InvalidOperationException("DB_CONNECTION environment variable is not set.");
    }
    Console.WriteLine($"This is the conn str: {connectionString}");
    options.UseSqlServer(connectionString);
});

builder.Services.AddTransient<UserService>();

builder.Services.AddControllers().AddJsonOptions(options =>
{
    options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
});

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();


//----------------------------------------

var app = builder.Build();

app.MapControllers();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.Run();