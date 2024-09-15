using System.Net;
using System.Text.Json.Serialization;
using dotenv.net;
using FluentValidation;
using HealthHub.Source.Config;
using HealthHub.Source.Data;
using HealthHub.Source.Filters.Error;
using HealthHub.Source.Helpers.Extensions;
using HealthHub.Source.Hubs;
using HealthHub.Source.Services;
using HealthHub.Source.Validation;
using HealthHub.Source.Validation.AppointmentValidation;
using HealthHub.Source.Validation.UserValidation;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json.Converters;
using Org.BouncyCastle.Asn1.X509.Qualified;
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

  builder.Services.AddCors(
    (options) =>
    {
      options.AddPolicy(
        "AllowSpecificOrigin",
        b =>
        {
          var config = new AppConfig(builder.Configuration);
          Log.Logger.Information($"\n\nALlowedOrigins: {config.AllowedOrigins}");

          b.WithOrigins(config.AllowedOrigins).AllowAnyMethod().AllowAnyHeader().AllowCredentials();
        }
      );
    }
  );

  /*
      Add Services to the Container
  */

  // Configure authentication with JWT and Auth0
  // 1. Set JwtBearer as the default authentication and challenge schemes
  // 2. Configure JwtBearer options with Auth0 settings
  builder
    .Services.AddAuthentication(options =>
    {
      // options.DefaultAuthenticateScheme = CookieAuthenticationDefaults.AuthenticationScheme;
      // options.DefaultChallengeScheme = CookieAuthenticationDefaults.AuthenticationScheme;
      // options.DefaultSignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
      options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
      options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    })
    // .AddCookie(
    //   CookieAuthenticationDefaults.AuthenticationScheme,
    //   option =>
    //   {
    //     var appConfig = new AppConfig(builder.Configuration);

    //     option.Cookie.HttpOnly = true;
    //     option.Cookie.SecurePolicy =
    //       (appConfig.IsProduction ?? false)
    //         ? CookieSecurePolicy.Always
    //         : CookieSecurePolicy.SameAsRequest;
    //     option.Cookie.SameSite = SameSiteMode.None;

    //     // option.Events.OnRedirectToLogin = context =>
    //     // {
    //     //   context.Response.StatusCode = StatusCodes.Status401Unauthorized;
    //     //   Console.WriteLine($"{context.Response}");
    //     //   return Task.CompletedTask;
    //     // };
    //   }
    // );
    .AddJwtBearer(options =>
    {
      var appConfig = new AppConfig(builder.Configuration);
      options.Authority = $"https://{appConfig.Auth0Domain}/";
      options.Audience = appConfig.Auth0Audience;
      options.RequireHttpsMetadata = appConfig.IsProduction ?? false;

      // Log.Logger.Information($"\nOrigins: {string.Join(",", appConfig.AllowedOrigins)}");
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
  builder.Services.AddValidatorsFromAssemblyContaining<EditProfileDtoValidator>();

  // Register the App Configuration Service
  builder.Services.AddSingleton<AppConfig>(provider =>
  {
    var config = provider.GetRequiredService<IConfiguration>();
    return new AppConfig(config);
  });

  // This service allows you to access the HttpContext in classes that
  // are not directly part of the HTTP request pipeline
  builder.Services.AddHttpContextAccessor();

  // Register the signalr service for realtime comms
  builder.Services.AddSignalR();

  // Register Services
  builder.Services.AddTransient<UserService>();
  builder.Services.AddTransient<DoctorService>();
  builder.Services.AddTransient<PatientService>();
  builder.Services.AddTransient<AdminService>();

  builder.Services.AddTransient<ChatService>();

  builder.Services.AddTransient<AppointmentService>();

  builder.Services.AddTransient<SpecialityService>();
  builder.Services.AddTransient<DoctorSpecialityService>();

  builder.Services.AddTransient<AuthService>();
  builder.Services.AddTransient<Auth0Service>();

  builder.Services.AddTransient<EmailService>();
  builder.Services.AddTransient<FileService>();
  builder.Services.AddTransient<RenderingService>();

  builder.Services.AddSingleton<UserConnection>();

  // This line registers the Lazy<T> type with the DI container to enable lazy loading for services.
  builder.Services.AddTransient(typeof(Lazy<>), typeof(Lazy<>));

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

  builder
    .Services.AddRazorPages()
    .AddRazorOptions(options =>
    {
      options.ViewLocationFormats.Add("/Source/Views/{0}.cshtml");
    });

  // Close and Flush Serilog when the application exits
  AppDomain.CurrentDomain.ProcessExit += (s, e) => Log.CloseAndFlush();

  //----------------------------------------
}

var app = builder.Build();


{
  // app.UseExceptionHandler("/error"); // Exception handling endpoint

  app.UseSerilogRequestLogging(); // Enable Serilog Request Logging

  app.UseCors("AllowSpecificOrigin");

  // Middlewares
  app.UseCustomValidation(); // Register the Custom Validation Middleware
  app.UseCookieMiddleware(); // Register the Cookie Middleware

  app.UseAuthentication();
  app.UseAuthorization();

  app.MapControllers();
  app.MapHub<ChatHub>("/chathub"); // the chatting hub
  app.MapHub<NotificationHub>("/notificationhub"); // the endpoint where notifications will be sent and the client listens to

  if (app.Environment.IsDevelopment())
  {
    app.UseSwagger();
    app.UseSwaggerUI();
  }

  app.Run();
}
