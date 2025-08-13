using Domain.Entities;

namespace Domain.DTOs;

public record ProcessedEmailMessage(
    EmailMessage Message,
    Func<Task> DeleteAsync
);
