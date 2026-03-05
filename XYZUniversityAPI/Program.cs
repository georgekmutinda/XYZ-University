using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;
using XYZUniversityAPI.Application.Interfaces;
using XYZUniversityAPI.Application.Services;
using XYZUniversityAPI.Domain.Interfaces;
using XYZUniversityAPI.Infrastructure.Data;
using XYZUniversityAPI.Infrastructure.Messaging;
using XYZUniversityAPI.Infrastructure.Repositories;
using XYZUniversityAPI.Infrastructure.Security;
using XYZUniversityAPI.Middleware;
using StackExchange.Redis;
using XYZUniversityAPI.BackgroundServices; 
using RabbitMQ.Client;
using Serilog;
using Microsoft.AspNetCore.SignalR;
using XYZUniversityAPI.Infrastructure.Hubs; 

Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .CreateBootstrapLogger();

try
{
    Log.Information("Starting XYZ University API...");

    var builder = WebApplication.CreateBuilder(args);

    builder.Host.UseSerilog((context, services, configuration) => configuration
        .ReadFrom.Configuration(context.Configuration)
        .ReadFrom.Services(services)
        .Enrich.FromLogContext());

    #region JWT SETTINGS
    builder.Services.Configure<JwtSettings>(builder.Configuration.GetSection("JwtSettings"));
    var jwtSettings = builder.Configuration.GetSection("JwtSettings").Get<JwtSettings>()!;
    #endregion

    #region REDIS & CACHING
    var redisConnectionString = builder.Configuration["Redis:Connection"] ?? "localhost:6379";
    var redisConnection = ConnectionMultiplexer.Connect(redisConnectionString);
    builder.Services.AddSingleton<IConnectionMultiplexer>(redisConnection);

    builder.Services.AddStackExchangeRedisCache(options =>
    {
        options.Configuration = redisConnectionString;
        options.InstanceName = "XYZUniversity_";
    });
    #endregion

    #region SIGNALR
    builder.Services.AddSignalR();
    #endregion

    #region RABBITMQ
    builder.Services.AddSingleton(sp => new ConnectionFactory
    {
        HostName = builder.Configuration["RabbitMQ:Host"] ?? "localhost",
        Port = 5672,
        UserName = "guest",
        Password = "guest"
    });

    builder.Services.AddSingleton<IRabbitMqPublisher, RabbitMqPublisher>();
    #endregion

    builder.Services.AddHttpClient();
    builder.Services.AddHttpContextAccessor();

    #region AUTHENTICATION & AUTHORIZATION
    builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
        .AddJwtBearer(options =>
        {
            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                ValidIssuer = jwtSettings.Issuer,
                ValidAudience = jwtSettings.Audience,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings.SecretKey)),
                ClockSkew = TimeSpan.Zero
            };
        });

    builder.Services.AddAuthorization(options =>
    {
        options.AddPolicy("AdminOnly", policy => policy.RequireRole("Admin"));
    });
    #endregion

    #region DATABASE
    builder.Services.AddDbContext<AppDbContext>(options =>
        options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection"))
    );
    #endregion

    #region DEPENDENCY INJECTION
    builder.Services.AddScoped<IStudentRepository, StudentRepository>();
    builder.Services.AddScoped<IPaymentRepository, PaymentRepository>();
    builder.Services.AddScoped<IClientRepository, ClientRepository>();
    builder.Services.AddScoped<IStudentService, StudentService>();
    builder.Services.AddScoped<IPaymentService, PaymentService>();
    builder.Services.AddScoped<ITokenService, TokenService>();
    #endregion

    #region HOSTED SERVICE
    builder.Services.AddHostedService<PaymentNotificationConsumer>();
    #endregion

    #region CONTROLLERS & CORS
    builder.Services.AddControllers();
    builder.Services.AddCors(options =>
    {
        options.AddPolicy("AllowAll", policy =>
            policy.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader());
    });
    #endregion

    #region SWAGGER
    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddSwaggerGen(c =>
    {
        c.SwaggerDoc("v1", new OpenApiInfo { Title = "XYZ University API", Version = "v1.2" });
        c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
        {
            Name = "Authorization",
            Type = SecuritySchemeType.Http,
            Scheme = "Bearer",
            BearerFormat = "JWT",
            In = ParameterLocation.Header
        });
        c.AddSecurityRequirement(new OpenApiSecurityRequirement {
            { new OpenApiSecurityScheme { Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = "Bearer" } }, Array.Empty<string>() }
        });
    });
    #endregion

    var app = builder.Build();

    #region MIDDLEWARE PIPELINE
    app.UseSerilogRequestLogging(); 
    if (app.Environment.IsDevelopment())
    {
        app.UseSwagger();
        app.UseSwaggerUI();
    }
    app.UseMiddleware<TokenExpiryMiddleware>();
    app.UseMiddleware<ExceptionMiddleware>();
    app.UseCors("AllowAll");
    app.UseAuthentication();
    app.UseAuthorization();
    app.MapControllers();
    app.MapHub<PaymentHub>("/paymentHub");
    #endregion

    #region SEEDING & AUTO-MIGRATION
    using (var scope = app.Services.CreateScope())
    {
        var services = scope.ServiceProvider;
        try 
        {
            var context = services.GetRequiredService<AppDbContext>();
            
            // FIX: This line creates the tables if they don't exist before seeding
            context.Database.Migrate(); 

            ClientSeed.Seed(context);
            ClientSeed.SeedData(context);
        }
        catch (Exception ex)
        {
            Log.Error(ex, "An error occurred during migration or seeding.");
            // We don't throw here so the migration tool can finish its work
        }
    }
    #endregion

    app.Run();
}
catch (Exception ex)
{
    Log.Fatal(ex, "Application terminated unexpectedly");
}
finally
{
    Log.CloseAndFlush();
}
