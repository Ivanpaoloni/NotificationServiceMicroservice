# 📬 Notification Microservice

Microservicio de notificaciones desarrollado en ASP.NET Core 8, enfocado en simplicidad, extensibilidad y resiliencia.


✉️ Funcionalidades

Envío de emails utilizando System.Net.Mail (sin librerías externas como MailKit).

Soporte para SMS (estructura preparada, aún no implementado).

Encolado de notificaciones en memoria mediante ConcurrentQueue.

Política de reintentos con Polly para manejar errores temporales.

Worker background que procesa las notificaciones de forma asíncrona.


🧰 Arquitectura

Patrón Factory para instanciar dinamicamente el NotificationSender correspondiente al canal (email, SMS, etc).

Separación clara por responsabilidad mediante interfaces:
INotificationSender, INotificationSenderFactory, INotificationQueue

Preparado para escalar hacia colas persistentes como RabbitMQ o Azure Service Bus.


❤️ Observabilidad

Integración de Health Checks con endpoints:

/health: estado general del microservicio.

/dashboard: UI amigable para monitoreo de salud.

HealthCheck personalizado para validar configuración SMTP.


🔧 Testing & Debug

Endpoint opcional para listar notificaciones pendientes (modo desarrollo).

Posibilidad de dejar notificaciones encoladas manualmente para pruebas.


🏗️ Futuras mejoras

Persistencia de notificaciones (base de datos).

Retry con backoff exponencial.

Implementación de canales adicionales: SMS, Push, etc.

Middleware de trazabilidad o logging avanzado.
