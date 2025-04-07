using FluentValidation;
using NotificationService.Configuration;
using NotificationService.Services;
using NotificationService.Services.Interfaces;
using NotificationService.Validations;
using NotificationService.Middlewares;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using NotificationService.Healthchecks;
using HealthChecks.UI.Client;
using NotificationService.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

//Configure ServiceProvider
builder.Services.Configure<SmtpSettings>(builder.Configuration.GetSection("Smtp"));
builder.Services.Configure<NotificationSettings>(builder.Configuration.GetSection("Notification"));

builder.Services.AddScoped<EmailNotificationSender>();
builder.Services.AddScoped<SmsNotificationSender>();
builder.Services.AddSingleton<INotificationQueue, InMemoryNotificationQueue>();
builder.Services.AddHostedService<NotificationWorker>();

//Configure DI
builder.Services.AddScoped<INotificationSenderFactory, NotificationSenderFactory>();
builder.Services.AddScoped<INotificationService, NotificationService.Services.NotificationService>();

//Configure FluentValidation
builder.Services.AddScoped<IValidator<NotificationRequest>, NotificationRequestValidator>();

//Configure HealthChecks
builder.Services.AddHealthChecks().AddCheck<CustomSmtpHealthCheck>("smtp");
builder.Services.AddHealthChecksUI().AddInMemoryStorage();

var app = builder.Build();

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
