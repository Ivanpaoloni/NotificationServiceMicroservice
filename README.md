# 📬 Notification Microservice

Microservicio de notificaciones desarrollado en **ASP.NET Core 8**, enfocado en simplicidad, extensibilidad y resiliencia.

---

## ✉️ Funcionalidades

- Envío de emails utilizando `System.Net.Mail` (sin librerías externas como MailKit).
- Soporte preparado para SMS (estructura creada, aún no implementado).
- Encolado de notificaciones en memoria mediante `ConcurrentQueue`.
- Política de reintentos con **Polly**, incluyendo *backoff exponencial*.
- Worker en segundo plano que procesa las notificaciones de forma asíncrona.
- Persistencia de notificaciones en base de datos SQL con estado de envío.
- Estados posibles: `Pending`, `Processing`, `Sent`, `Failed`, `Cancelled`.

---

## 🧰 Arquitectura

- Patrón **Factory** para instanciar dinámicamente el `NotificationSender` correspondiente al canal (email, SMS, etc.).
- Separación clara por responsabilidad mediante interfaces:
  - `INotificationSender`
  - `INotificationSenderFactory`
  - `INotificationQueue`
- Preparado para escalar hacia colas persistentes como **RabbitMQ**, **Azure Service Bus**, etc.

---

## ❤️ Observabilidad

- Integración de **Health Checks** con endpoints:
  - `/health`: estado general del microservicio.
  - `/dashboard`: UI amigable para monitoreo de salud.
- HealthCheck personalizado para validar la configuración SMTP.

---

## 🔧 Testing & Debug

- Endpoint opcional para listar notificaciones pendientes (modo desarrollo).
- Posibilidad de dejar notificaciones encoladas manualmente para pruebas rápidas.

---

## ⏱️ Manejo de Reintentos

- Implementado mediante **Polly** con:
  - Reintentos automáticos hasta 3 veces.
  - **Backoff exponencial** entre intentos.
  - Logging detallado por intento y error.
- Las notificaciones que fallan se marcan como `Failed` en la base de datos, con conteo de reintentos.
- Las notificaciones que aún no fueron procesadas o fallaron, se vuelven a encolar si tienen menos de 3 reintentos.

---

## 🏗️ Futuras mejoras

- Retry con jitter aleatorio (para evitar picos simultáneos).
- Implementación de nuevos canales: SMS real, Push Notifications, etc.
- Integración con colas distribuidas (RabbitMQ, Azure Service Bus, Kafka).
- Middleware de trazabilidad y logging centralizado (Serilog, OpenTelemetry).
- Persistencia de la cola (para evitar pérdida de mensajes ante reinicios).

---

## 🚀 Ejecutar localmente

```bash
dotnet run --project NotificationService
