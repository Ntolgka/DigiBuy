using System.Text;
using System.Text.Json.Serialization;
using DigiBuy.Api.Middlewares;
using DigiBuy.Application.Mappings;
using DigiBuy.Application.Messaging;
using DigiBuy.Application.Services.Implementations;
using DigiBuy.Application.Services.Interfaces;
using DigiBuy.Application.Token;
using DigiBuy.Application.Validators.Create;
using DigiBuy.Application.Validators.Update;
using DigiBuy.Domain.Entities;
using DigiBuy.Domain.Enumerations;
using DigiBuy.Domain.Repositories;
using DigiBuy.Infrastructure.Data;
using DigiBuy.Infrastructure.Repositories;
using Hangfire;
using Hangfire.PostgreSql;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Serilog;
using FluentValidation;
using FluentValidation.AspNetCore;
using StackExchange.Redis;

var builder = WebApplication.CreateBuilder(args);

// Serilog
Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Debug()
    .WriteTo.Console()
    .WriteTo.File("Logs/digibuy.log", rollingInterval: RollingInterval.Day)
    .CreateLogger();

builder.Services.AddControllers().AddJsonOptions(options =>
{
    options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
    options.JsonSerializerOptions.WriteIndented = true;
    options.JsonSerializerOptions.PropertyNamingPolicy = null;
});

builder.Host.UseSerilog();

builder.Services.AddEndpointsApiExplorer();

// Swagger
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "DigiBuy", Version = "v1.0" });
    var securityScheme = new OpenApiSecurityScheme
    {
        Name = "DigiBuy",
        Description = "Enter JWT Bearer token **_only_**",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        Reference = new OpenApiReference
        {
            Id = JwtBearerDefaults.AuthenticationScheme,
            Type = ReferenceType.SecurityScheme
        }
    };
    c.AddSecurityDefinition(securityScheme.Reference.Id, securityScheme);
    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        { securityScheme, new string[] { } }
    });
});

// Authentication JWT
var jwtSettings = builder.Configuration.GetSection("JwtSettings").Get<JwtSettings>();
builder.Services.AddSingleton<JwtSettings>(jwtSettings);

builder.Services.AddAuthentication(options =>
    {
        options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
        options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    })
    .AddJwtBearer(options =>
    {
        options.RequireHttpsMetadata = true;
        options.SaveToken = true;
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = jwtSettings.Issuer,
            ValidAudience = jwtSettings.Audience,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(jwtSettings.Secret))
        };
    });

// DB Connection (PostgreSQL)
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("PostgresSqlConnection")).EnableSensitiveDataLogging());


// Hangfire Connection
builder.Services.AddHangfire(configuration => configuration
    .SetDataCompatibilityLevel(CompatibilityLevel.Version_180)
    .UseSimpleAssemblyNameTypeSerializer()
    .UseRecommendedSerializerSettings()
    .UsePostgreSqlStorage(builder.Configuration.GetConnectionString("HangfireConnection")));

builder.Services.AddHangfireServer();

// Redis
var redisConfig = new ConfigurationOptions();
redisConfig.DefaultDatabase = 0;
redisConfig.EndPoints.Add(builder.Configuration["Redis:Host"], Convert.ToInt32(builder.Configuration["Redis:Port"]));
builder.Services.AddStackExchangeRedisCache(opt =>
{
    opt.ConfigurationOptions = redisConfig;
    opt.InstanceName = builder.Configuration["Redis:InstanceName"];
});

// AutoMapper
builder.Services.AddAutoMapper(typeof(AutoMapperProfile).Assembly);

// SMTP
builder.Services.Configure<SmtpSettings>(builder.Configuration.GetSection("SMTPConfig"));

// Generic Repository
builder.Services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));

// Unit Of Work
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();

// EF Identity
builder.Services.AddIdentity<User, IdentityRole>(options =>
    {
        options.Password.RequireDigit = true;
        options.Password.RequireLowercase = true;
        options.Password.RequireUppercase = true;
        options.Password.RequireNonAlphanumeric = true;
        options.Password.RequiredLength = 6;
        options.Password.RequiredUniqueChars = 1;
        options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);
        options.Lockout.MaxFailedAccessAttempts = 5;
        options.User.RequireUniqueEmail = true;
    })
    .AddEntityFrameworkStores<AppDbContext>()
    .AddDefaultTokenProviders();

// Authorization
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("AdminOnly", policy =>
        policy.RequireRole(UserRole.Admin.ToString()));

    options.AddPolicy("UserOnly", policy =>
        policy.RequireRole(UserRole.User.ToString()));
});

// FluentValidation
builder.Services.AddFluentValidationAutoValidation();
builder.Services.AddFluentValidationClientsideAdapters();

builder.Services.AddValidatorsFromAssemblyContaining<CreateCategoryDTOValidator>();
builder.Services.AddValidatorsFromAssemblyContaining<UpdateCategoryDTOValidator>();

// Services
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IProductService, ProductService>();
builder.Services.AddScoped<ICategoryService, CategoryService>();
builder.Services.AddScoped<IOrderService, OrderService>();
builder.Services.AddScoped<ICouponService, CouponService>();
builder.Services.AddScoped<IOrderDetailService, OrderDetailService>();
builder.Services.AddScoped<JwtTokenService>();
builder.Services.AddScoped<ICheckoutService, CheckoutService>();
builder.Services.AddSingleton<IEmailService, EmailService>();
builder.Services.AddSingleton<RabbitMqProducer>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseMiddleware<RequestResponseLoggingMiddleware>();
app.UseMiddleware<ErrorHandlerMiddleware>(); 

// Hangfire Dashboard
app.UseHangfireDashboard();

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

using (var scope = app.Services.CreateScope())
{
    var emailService = scope.ServiceProvider.GetRequiredService<IEmailService>();
    RecurringJob.AddOrUpdate(
        "process-email-jobs",
        () => emailService.ProcessEmailJobs(),
        "*/5 * * * *"); // Adjust the schedule as needed
}

Log.Information("Starting up");

app.Run();

Log.CloseAndFlush();