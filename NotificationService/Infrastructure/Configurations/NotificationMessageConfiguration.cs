using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NotificationService.Domain.Entities;

namespace NotificationService.Infrastructure.Configurations
{
    public class NotificationMessageConfiguration : IEntityTypeConfiguration<NotificationMessage>
    {
        public void Configure(EntityTypeBuilder<NotificationMessage> builder)
        {
            builder.ToTable("NotificationMessages");

            builder.HasKey(n => n.Id);
            builder.Property(n => n.Channel).IsRequired().HasColumnType("smallint");
            builder.Property(n => n.Recipient).IsRequired().HasColumnType("nvarchar(200)");
            builder.Property(n => n.Subject).IsRequired().HasColumnType("nvarchar(200)");
            builder.Property(n => n.Message).IsRequired().HasColumnType("nvarchar(max)");
            builder.Property(n => n.Status).IsRequired().HasColumnType("smallint");
            builder.Property(n => n.RetryCount).IsRequired().HasColumnType("smallint");
            builder.Property(n => n.CreatedAt).IsRequired().HasColumnType("datetime");
            builder.Property(n => n.SentAt).HasColumnType("datetime");
        }
    }
}
