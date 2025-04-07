using NotificationService.Services.Interfaces;

namespace NotificationService.Services
{
    public class NotificationWorker : BackgroundService
    {
        private readonly INotificationQueue _queue;
        private readonly IServiceScopeFactory _scopeFactory;

        public NotificationWorker(INotificationQueue queue, IServiceScopeFactory scopeFactory)
        {
            _queue = queue;
            _scopeFactory = scopeFactory;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                if (_queue.TryDequeue(out var request))
                {
                    using var scope = _scopeFactory.CreateScope();
                    var senderFactory = scope.ServiceProvider.GetRequiredService<INotificationSenderFactory>();
                    try
                    {
                        var sender = senderFactory.GetSender(request!.Channel);
                        await sender.SendAsync(request.Recipient, request.Subject, request.Message);
                    }
                    catch (Exception ex)
                    {
                        // Loguear error o implementar retry
                        Console.WriteLine($"Error al enviar notificación: {ex.Message}");
                    }
                }

                await Task.Delay(100, stoppingToken); // Pequeño delay para no saturar el CPU
            }
        }
    }

}
