using System.Diagnostics;
using Application.ProcessEmail;
using Domain.DTOs;
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
        var count = 0;

        try
        {
            await foreach (ProcessedEmailMessage item in emailReader.ReadEmailsAsync(cancellationToken))
            {
                logger.LogInformation("✉️ Subject: {Subject}", item.Message.Subject);
                count++;
            }

            logger.LogInformation("✔️ Stream completed. Logged {Count} emails.", count);
        }
        catch (OperationCanceledException ex)
        {
            logger.LogWarning(ex, "⏹️ Processing canceled.");
        }
        catch (System.Exception ex)
        {
            logger.LogError(ex, "❌ Error while processing emails.");
        }
        finally
        {
            logger.LogInformation("🛑 EmailProcessorWorker stopping.");
            appLifetime.StopApplication();
        }
    }

    public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;
}
