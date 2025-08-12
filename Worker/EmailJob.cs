using Microsoft.Extensions.Logging;
using Application.ProcessEmail;
using Domain.Interfaces;
using TickerQ.Utilities.Base;

namespace Worker;

public class EmailJob(
    ILogger<EmailJob> logger,
    IEmailReader emailReader,
    IProcessEmailHandler handler
)
{
    [TickerFunction(functionName: nameof(ProcessEmailsAsync), cronExpression: "* * * * *")]
    public async Task ProcessEmailsAsync(CancellationToken ct)
    {
        logger.LogInformation("⏱️ TickerQ started: processing emails...");

        var emails = await emailReader.ReadEmailsAsync(ct);

        foreach (var e in emails)
            await handler.Handle(new(e.Subject, e.AttachmentName, e.AttachmentBytes), ct);

        logger.LogInformation("✔️ Processed {Count} emails.", emails?.Count() ?? 0);
    }
}
