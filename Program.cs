using System.Text.Json.Serialization;
using dotenv.net;
using FluentValidation;
using HealthHub.Source.Config;
using HealthHub.Source.Data;
using HealthHub.Source.Filters.Error;
using HealthHub.Source.Helpers.Extensions;
using HealthHub.Source.Services;
using HealthHub.Source.Validation;
using HealthHub.Source.Validation.AppointmentValidation;
using HealthHub.Source.Validation.UserValidation;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json.Converters;
using Serilog;

// Load Environment Variables
DotEnv.Load(options: new DotEnvOptions(ignoreExceptions: false));

var builder = WebApplication.CreateBuilder(args);


{
  // Configure Serilog with appropriate sinks
  Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Debug() // Set the minimum log level to Debug
    .WriteTo.Console() // Write logs to the console
    .WriteTo.File("Logs/HealthHub.log", rollingInterval: RollingInterval.Day) // Write logs to a file
    .WriteTo.Seq("http://localhost:5341/") // Write logs to Seq
    .CreateLogger();

  Log.Information("Application Starting...");

  // Configure Serilog to capture logs from application host
  builder.Host.UseSerilog();

  // Database Service
  builder.Services.AddDbContext<ApplicationContext>(
    (serviceProvider, options) =>
    {
      var appConfig = serviceProvider.GetRequiredService<AppConfig>();
      var connectionString = appConfig.DatabaseConnection;
      if (string.IsNullOrEmpty(connectionString))
      {
        throw new InvalidOperationException("DB_CONNECTION environment variable is not set.");
      }
      Log.Information($"This is the conn str: {connectionString}");
      options.UseSqlServer(connectionString);
    }
  );

  builder.Services.Configure<ApiBehaviorOptions>(options =>
  {
    options.SuppressModelStateInvalidFilter = true;
  });

  /*
      Add Services to the Container
  */

  // Configure authentication with JWT and Auth0
  // 1. Set JwtBearer as the default authentication and challenge schemes
  // 2. Configure JwtBearer options with Auth0 settings
  builder
    .Services.AddAuthentication(options =>
    {
      options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
      options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    })
    .AddJwtBearer(options =>
    {
      var appConfig = new AppConfig(builder.Configuration);
      options.Authority = $"https://{appConfig.Auth0Domain}/";
      options.Audience = appConfig.Auth0Audience;
      options.RequireHttpsMetadata = appConfig.IsProduction ?? false;

      Log.Logger.Information($"\nAudience: {options.Audience}");
      Log.Logger.Information($"\nAuthority: {options.Authority}");
      Log.Logger.Information($"\nClientId: {appConfig.Auth0ClientId}");
      Log.Logger.Information($"\nClientSecret: {appConfig.Auth0ClientSecret}");

      // Configure Token Validation Parameters
      options.TokenValidationParameters = new TokenValidationParameters
      {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = appConfig.Auth0Authority,
        ValidAudience = appConfig.Auth0Audience
      };
    });

  // Configure Authorization
  builder.Services.AddAuthorization(options =>
  {
    options.AddPolicy("Admin", policy => policy.RequireRole("Admin"));
    options.AddPolicy("Doctor", policy => policy.RequireRole("Doctor"));
    options.AddPolicy("Patient", policy => policy.RequireRole("Patient"));
  });

  // Controllers & Views Service
  builder.Services.AddControllersWithViews(options =>
  {
    // Register the global exception handler filer
    // options.Filters.Add<GlobalExceptionFilter>();
  });

  // Register Validation Services
  builder.Services.AddValidatorsFromAssemblyContaining<RegisterUserDtoValidator>();
  builder.Services.AddValidatorsFromAssemblyContaining<CreateAppointmentDtoValidator>();
  builder.Services.AddValidatorsFromAssemblyContaining<EditAppointmentDtoValidator>();

  // Register the App Configuration Service
  builder.Services.AddSingleton<AppConfig>(provider =>
  {
    var config = provider.GetRequiredService<IConfiguration>();
    return new AppConfig(config);
  });

  // Register Services
  builder.Services.AddTransient<UserService>();
  builder.Services.AddTransient<DoctorService>();
  builder.Services.AddTransient<PatientService>();
  builder.Services.AddTransient<AdminService>();

  builder.Services.AddTransient<AppointmentService>();
  builder.Services.AddTransient<AvailabilityService>();

  builder.Services.AddTransient<SpecialityService>();
  builder.Services.AddTransient<DoctorSpecialityService>();

  builder.Services.AddTransient<AuthService>();
  builder.Services.AddTransient<Auth0Service>();

  builder.Services.AddTransient<EmailService>();
  builder.Services.AddTransient<FileService>();
  builder.Services.AddTransient<RenderingService>();

  builder
    .Services.AddControllers()
    .AddNewtonsoftJson(options =>
    {
      options.SerializerSettings.ReferenceLoopHandling = Newtonsoft
        .Json
        .ReferenceLoopHandling
        .Ignore;
      options.SerializerSettings.Converters.Add(
        new Newtonsoft.Json.Converters.StringEnumConverter()
      );
      options.SerializerSettings.Converters.Add(new IsoDateTimeConverter());
    });

  builder.Services.AddEndpointsApiExplorer();
  builder.Services.AddSwaggerGen(options =>
  {
    var xmlFile = "HealthHub.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    options.IncludeXmlComments(xmlPath);
  });

  // Close and Flush Serilog when the application exits
  AppDomain.CurrentDomain.ProcessExit += (s, e) => Log.CloseAndFlush();

  //----------------------------------------
}

var app = builder.Build();


{
  app.UseSerilogRequestLogging(); // Enable Serilog Request Logging

  // app.UseExceptionHandler("/error"); // Exception handling endpoint

  app.UseCustomValidation(); // Register the Custom Validation Middleware

  app.UseAuthentication();
  app.UseAuthorization();

  app.MapControllers();

  if (app.Environment.IsDevelopment())
  {
    app.UseSwagger();
    app.UseSwaggerUI();
  }

  app.Run();
}
