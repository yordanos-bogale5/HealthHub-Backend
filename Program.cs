using dotenv.net;
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

var app = builder.Build();

app.MapControllers();

app.Run();