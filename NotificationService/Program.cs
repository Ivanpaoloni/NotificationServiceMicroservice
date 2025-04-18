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
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

//configure secrets
builder.Configuration.AddUserSecrets<Program>();

//Configure DbContext
builder.Services.AddDbContext<NotificationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
    });

//Configure ServiceProvider
builder.Services.Configure<SmtpSettings>(builder.Configuration.GetSection("Smtp"));
builder.Services.Configure<NotificationSettings>(builder.Configuration.GetSection("Notification"));
builder.Services.Configure<TwilioSettings>(builder.Configuration.GetSection("Twilio"));

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
builder.Services.AddHealthChecks().AddDbContextCheck<NotificationDbContext>("SQL Database");
builder.Services.AddHealthChecks().AddCheck<NotificationWorkerHealthCheck>("NotificationWorker"); 
builder.Services.AddHealthChecks().AddCheck<TwilioHealthCheck>("Twilio_SMS");

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

//connectionString validation
try
{
    using var scope = app.Services.CreateScope();
    var dbContext = scope.ServiceProvider.GetRequiredService<NotificationDbContext>();

    if (!dbContext.Database.CanConnect())
    {
        Console.WriteLine("No se puede conectar con la base de datos. Verificá la cadena de conexión.");
        return;
    }
    //dbContext.Database.Migrate();
}
catch (Exception ex)
{
    Console.WriteLine($"Error de inicio: {ex.Message}");
    return;
}

app.Run();