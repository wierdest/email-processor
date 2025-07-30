
using Microsoft.Extensions.Logging;

namespace Application.ProcessEmail;

public class ProcessEmailHandler(ILogger<ProcessEmailHandler> logger) : IProcessEmailHandler
{
    public Task Handle(ProcessEmailCommand command, CancellationToken cancellationToken)
    {
        logger.LogInformation("📥 Processing email:");
        logger.LogInformation("Subject: {Subject}", command.Subject);
        logger.LogInformation("Attachment: {AttachmentName}", command.AttachmentName);
        logger.LogInformation("Size: {Size} bytes", command.AttachmentBytes.Length);

        return Task.CompletedTask;
    }
}
