using NotificationService.Configuration;
using NotificationService.Services;
using NotificationService.Services.Interfaces;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

//Configure ServiceProvider
builder.Services.Configure<SmtpSettings>(builder.Configuration.GetSection("Smtp"));
builder.Services.AddScoped<EmailNotificationSender>();
builder.Services.AddScoped<SmsNotificationSender>();
builder.Services.AddScoped<INotificationSenderFactory, NotificationSenderFactory>();
builder.Services.AddScoped<INotificationService,NotificationService.Services.NotificationService>();

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();

app.UseHttpsRedirection();

app.UseAuthorization();
app.MapControllers();
app.Run();
