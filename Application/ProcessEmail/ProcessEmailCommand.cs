namespace Application.ProcessEmail;

public record class ProcessEmailCommand( string Subject, string AttachmentName, byte[] AttachmentBytes);
