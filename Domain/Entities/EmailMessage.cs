using System;

namespace Domain.Entities;

public class EmailMessage
{
    public string Id { get; init; } = Guid.NewGuid().ToString();
    public string Subject { get; init; } = string.Empty;
    public string AttachmentName { get; init; } = string.Empty;
    public byte[] AttachmentBytes { get; init; } = [];
    public DateTime ProcessedAt { get; init; } = DateTime.UtcNow;
}
