using FluentValidation;
using HealthChecks.UI.Client;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.EntityFrameworkCore;
using NotificationService.Configuration;
using NotificationService.Healthchecks;
using NotificationService.Infrastructure;
using NotificationService.Middlewares;
using NotificationService.Models;
using NotificationService.Models.Dtos;
using NotificationService.Services;
using NotificationService.Services.Interfaces;
using NotificationService.Validations;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

//Configure DbContext
builder.Services.AddDbContext<NotificationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));


//Configure ServiceProvider
builder.Services.Configure<SmtpSettings>(builder.Configuration.GetSection("Smtp"));
builder.Services.Configure<NotificationSettings>(builder.Configuration.GetSection("Notification"));

//Configure AutoMapper
builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

//configure services
builder.Services.AddScoped<EmailNotificationSender>();
builder.Services.AddScoped<SmsNotificationSender>();
builder.Services.AddSingleton<INotificationQueue, InMemoryNotificationQueue>();
builder.Services.AddHostedService<NotificationWorker>();

//Configure DI
builder.Services.AddScoped<INotificationSenderFactory, NotificationSenderFactory>();
builder.Services.AddScoped<INotificationService, NotificationService.Services.NotificationService>();

//Configure FluentValidation
builder.Services.AddScoped<IValidator<NotificationRequestDto>, NotificationRequestDtoValidator>();

//worker status service
builder.Services.AddSingleton<WorkerStatusService>();

//Configure HealthChecks
builder.Services.AddHealthChecks().AddCheck<CustomSmtpHealthCheck>("smtp");
builder.Services.AddHealthChecks().AddCheck<CustomDbHealthCheck>("SQL Database");
builder.Services.AddHealthChecks().AddCheck<NotificationWorkerHealthCheck>("NotificationWorker");
builder.Services.AddHealthChecksUI().AddInMemoryStorage();

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
        policy.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod());
});


var app = builder.Build();

app.UseCors("AllowAll");
app.UseExceptionHandling();

//HealthCheck Middleware
app.UseHealthChecks("/health", new HealthCheckOptions { ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse });
app.UseHealthChecksUI(config => config.UIPath = "/dashboard"); // URL donde se verá el dashboard});

app.UseSwagger();
app.UseSwaggerUI();

app.UseHttpsRedirection();

app.UseAuthorization();
app.MapControllers();
app.Run();
