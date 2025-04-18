# 📬 Notification Microservice

Microservicio de notificaciones desarrollado en **ASP.NET Core 8**, enfocado en simplicidad, extensibilidad y resiliencia.

---

## ✉️ Funcionalidades

- Envío de emails utilizando `System.Net.Mail` (sin librerías externas como MailKit).
- Envío de **SMS reales** mediante integración con **Twilio**.
- Encolado de notificaciones en memoria mediante `ConcurrentQueue`.
- Política de reintentos con **Polly**, incluyendo *backoff exponencial*.
- Worker en segundo plano que procesa las notificaciones de forma asíncrona.
- Persistencia de notificaciones en base de datos SQL con estado de envío.
- Estados posibles: `Pending`, `Processing`, `Sent`, `Failed`, `Cancelled`.

---
![image](https://github.com/user-attachments/assets/7969b5ec-a84a-4a90-ba94-51f15580781d)

## 🧰 Arquitectura

- Patrón **Factory** para instanciar dinámicamente el `NotificationSender` correspondiente al canal (`EmailSender`, `SmsSender`, etc.).
- Separación clara por responsabilidad mediante interfaces:
  - `INotificationSender`
  - `INotificationSenderFactory`
  - `INotificationQueue`
- Preparado para escalar hacia colas persistentes como **RabbitMQ**, **Azure Service Bus**, etc.

---

## 🔐 Manejo de secretos

- Las claves sensibles (como `Twilio:AccountSid`, `Twilio:AuthToken`, `SMTP` config, etc.) **no deben estar en `appsettings.json`**.
- Utilizá **User Secrets** en desarrollo:

```bash
dotnet user-secrets set "Twilio:AccountSid" "TU_SID"
dotnet user-secrets set "Twilio:AuthToken" "TU_TOKEN"
dotnet user-secrets set "Twilio:FromPhone" "+123456789"
```

## 🌐 Variables de entorno (producción)

O usar servicios como:

Azure Key Vault

AWS Secrets Manager

HashiCorp Vault

⚠️ Si accidentalmente subís un secreto, GitHub puede bloquear el push. Se recomienda hacer `git rebase -i` o `git filter-repo` para eliminarlo del historial.

## ❤️ Observabilidad

✔️ Health Checks integrados:

`/health`: estado general del microservicio.

`/dashboard`: vista web para monitoreo de salud.

HealthChecks personalizados para:

Configuración SMTP válida.

Conectividad con Twilio (opcional).

Preparado para extender con:

Serilog

OpenTelemetry

Prometheus + Grafana

## 🔧 Testing & Debug

Endpoint opcional en desarrollo para inspeccionar notificaciones pendientes.

Posibilidad de enviar notificaciones manualmente desde Swagger o Postman.

Control total sobre la inyección de fallas y verificación de reintentos.

## ⏱️ Manejo de Reintentos
Implementado mediante Polly:

Hasta 3 reintentos automáticos.

Backoff exponencial entre cada intento.

Logging detallado de cada intento y su error.

Las notificaciones con error se marcan como Failed y se persisten con cantidad de intentos.

Las fallidas con menos de 3 intentos vuelven a encolarse para reintentar luego.

## 🏗️ Futuras mejoras
Retry con jitter aleatorio (para evitar reintentos simultáneos).

Nuevos canales: Push Notifications, WhatsApp, etc.

Integración con colas distribuidas: RabbitMQ, Azure Service Bus, Kafka.

Persistencia de la cola en disco o base de datos.

Dashboard en tiempo real con métricas e historial.

Middleware de trazabilidad para logs correlacionados (RequestId, etc.).

## 🚀 Ejecutar localmente

`dotnet run --project NotificationService`

✅ Asegurate de tener configurados los secrets previamente (SMTP y/o Twilio).
