using System.Diagnostics;
using Application.ProcessEmail;
using Domain.Interfaces;

namespace Worker;

public class EmailProcessorWorker(
    ILogger<EmailProcessorWorker> logger,
    IEmailReader emailReader,
    IProcessEmailHandler handler
) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        logger.LogInformation("📨 EmailProcessorWorker started.");

        try
        {
            var emailMessages = await emailReader.ReadEmailsAsync(stoppingToken);

            foreach (var email in emailMessages)
            {
                var command = new ProcessEmailCommand(
                    Subject: email.Subject,
                    AttachmentName: email.AttachmentName,
                    AttachmentBytes: email.AttachmentBytes
                );

                await handler.Handle(command, stoppingToken);

            }

            logger.LogInformation("✔️ Processed {Count} emails.", emailMessages?.Count() ?? 0);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "❌ Error while processing emails");
        }
        logger.LogInformation("🛑 EmailProcessorWorker stopping.");
    }
}
