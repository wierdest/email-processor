using System.Diagnostics;
using Application.ProcessEmail;
using Domain.Interfaces;

namespace Worker;
public class EmailProcessorWorker(
    ILogger<EmailProcessorWorker> logger,
    IEmailReader emailReader,
    IProcessEmailHandler handler,
    IHostApplicationLifetime appLifetime
) : IHostedService
{
    public async Task StartAsync(CancellationToken cancellationToken)
    {
        logger.LogInformation("📨 EmailProcessorWorker started.");

        try
        {
            var emailMessages = await emailReader.ReadEmailsAsync(cancellationToken);

            foreach (var email in emailMessages)
            {
                var command = new ProcessEmailCommand(
                    Subject: email.Subject,
                    AttachmentName: email.AttachmentName,
                    AttachmentBytes: email.AttachmentBytes
                );

                await handler.Handle(command, cancellationToken);
            }

            logger.LogInformation("✔️ Processed {Count} emails.", emailMessages?.Count() ?? 0);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "❌ Error while processing emails");
        }

        logger.LogInformation("🛑 EmailProcessorWorker stopping.");
        appLifetime.StopApplication();
    }

    public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;
}
