using Domain.Entities;
using Domain.Interfaces;
using Infrastructure.Email.Settings;
using MailKit;
using MailKit.Net.Imap;
using MailKit.Search;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Logging;
using MimeKit;
using Domain.DTOs;
using System.Runtime.CompilerServices;

namespace Infrastructure.Email;


public class IMAPEmailReader(
    ILogger<IMAPEmailReader> logger,
    IOptions<IMAPSettings> options
    ) : IEmailReader
{
    private readonly IMAPSettings _settings = options.Value;
    private readonly ILogger<IMAPEmailReader> _logger = logger;
    public async IAsyncEnumerable<ProcessedEmailMessage> ReadEmailsAsync(
        [EnumeratorCancellation] CancellationToken cancellationToken)
    {
        using var client = new ImapClient();

        _logger.LogInformation("📥 Connecting to IMAP server {Host}:{Port}", _settings.Host, _settings.Port);
        await client.ConnectAsync(_settings.Host, _settings.Port, true, cancellationToken);
        await client.AuthenticateAsync(_settings.Username, _settings.Password, cancellationToken);

        var inbox = client.Inbox;
        await inbox.OpenAsync(FolderAccess.ReadWrite, cancellationToken);

        var uids = await inbox.SearchAsync(SearchQuery.All, cancellationToken);
        _logger.LogInformation("🔍 Found {Count} messages.", uids.Count);

        foreach (var uid in uids)
        {
            var mime = await inbox.GetMessageAsync(uid, cancellationToken);
            // só o placeholder
            var email = new EmailMessage
            {
                Subject = string.IsNullOrWhiteSpace(mime.Subject) ? "(No Subject)" : mime.Subject
            };

            yield return new ProcessedEmailMessage(
                email,
                DeleteAsync: async () =>
                {
                    _logger.LogInformation("🧹 Marking email UID {Uid} for deletion: {Subject}", uid, email.Subject);
                    await inbox.AddFlagsAsync(uid, MessageFlags.Deleted, silent: true, cancellationToken);
                });
        }

        _logger.LogInformation("🗑️ Expunging deleted messages…");
        await inbox.ExpungeAsync(cancellationToken);
        _logger.LogInformation("✅ Expunge complete.");

        if (client.IsConnected)
        {
            await client.DisconnectAsync(true, cancellationToken);
            _logger.LogInformation("🔌 Disconnected from IMAP server.");
        }
    }
}
