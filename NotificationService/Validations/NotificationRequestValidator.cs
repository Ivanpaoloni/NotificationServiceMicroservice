using FluentValidation;
using NotificationService.Domain.Enums;
using NotificationService.Models;

namespace NotificationService.Validations
{
    public class NotificationRequestValidator : AbstractValidator<NotificationRequest>
    {
        public NotificationRequestValidator()
        {
            RuleFor(x => x.Recipient)
                .NotEmpty()
                .WithMessage("Recipient is required.");

            RuleFor(x => x.Subject)
                .NotEmpty()
                .WithMessage("Subject is required.");

            RuleFor(x => x.Message)
                .NotEmpty()
                .WithMessage("Message is required.");

            RuleFor(x => x.Channel)
                .IsInEnum()
                .WithMessage("Invalid notification channel.");

            When(x => x.Channel == NotificationChannelTypeEnum.Email, () =>
            {
                RuleFor(x => x.Recipient)
                    .EmailAddress()
                    .WithMessage("Invalid email address.");
            });

            When(x => x.Channel == NotificationChannelTypeEnum.SMS, () =>
            {
                RuleFor(x => x.Recipient)
                    .Matches(@"^\+?[1-9]\d{10,15}$")
                    .WithMessage("Invalid phone number.");
            });
        }
    }
}
