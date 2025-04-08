# 📬 Notification Microservice

Microservicio de notificaciones desarrollado con **ASP.NET Core 8**, enfocado en **simplicidad**, **extensibilidad** y **resiliencia**.

---

## ✉️ Funcionalidades

- ✅ Envío de **emails** utilizando `System.Net.Mail` (sin dependencias externas como MailKit).  
- 🕐 **Procesamiento asíncrono** mediante un `BackgroundService` (worker).
- 📥 Encolado de notificaciones en memoria con `ConcurrentQueue`.
- 🔁 **Política de reintentos** con [Polly](https://github.com/App-vNext/Polly) para fallos temporales.
- 💾 Persistencia en **base de datos SQL** con control de estado (`Pending`, `Sent`, `Failed`).
- 🧱 Estructura preparada para **SMS** (aún no implementado).

---

## 🧰 Arquitectura

- 🏭 Patrón **Factory** para instanciar dinámicamente el `NotificationSender` según canal (Email, SMS, etc).
- 💡 Separación clara por responsabilidades mediante interfaces como:
  - `INotificationSender`
  - `INotificationSenderFactory`
  - `INotificationQueue`
- 🔌 Preparado para escalar a colas externas como **RabbitMQ**, **Azure Service Bus**, etc.

---

## ❤️ Observabilidad

- 🔍 **Health Checks** expuestos vía endpoints:
  - `/health`: Estado general del microservicio.
  - `/dashboard`: UI amigable para monitoreo.
- ⚙️ HealthCheck **custom** para validar configuración SMTP.

---

## 🧪 Testing & Debug

- 🧾 Endpoint opcional para **listar notificaciones pendientes** (modo desarrollo).
- 🧪 Posibilidad de insertar notificaciones manualmente en la cola para pruebas.

---

## 🏗️ Futuras mejoras

- 📈 Retry con **backoff exponencial**.
- 📲 Implementación de canales adicionales: **SMS**, **Push Notifications**, etc.
- 🔎 Middleware para trazabilidad (trace ID, correlation ID).
- 📊 Integración con **logging avanzado** (Ej. Serilog, OpenTelemetry).

---

> Diseñado con foco en resiliencia y escalabilidad para integrarse fácilmente a cualquier arquitectura basada en microservicios.
