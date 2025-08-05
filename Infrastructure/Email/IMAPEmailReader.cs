using Domain.Entities;
using Domain.Interfaces;
using Infrastructure.Email.Settings;
using MailKit;
using MailKit.Net.Imap;
using MailKit.Search;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Logging;
using MimeKit;

namespace Infrastructure.Email;

public class IMAPEmailReader(
    ILogger<IMAPEmailReader> logger,
    IOptions<IMAPSettings> options
    ) : IEmailReader
{
    private readonly IMAPSettings _settings = options.Value;
    private readonly ILogger<IMAPEmailReader> _logger = logger;
    public async Task<IEnumerable<EmailMessage>> ReadEmailsAsync(CancellationToken cancellationToken)
    {
        var result = new List<EmailMessage>();

        using var client = new ImapClient();

        try
        {
            _logger.LogInformation("📥 Connecting to IMAP server {Host}:{Port}", _settings.Host, _settings.Port);

            await client.ConnectAsync(_settings.Host, _settings.Port, true, cancellationToken);

            await client.AuthenticateAsync(_settings.Username, _settings.Password, cancellationToken);
            var inbox = client.Inbox;

            await inbox.OpenAsync(FolderAccess.ReadWrite, cancellationToken);

            var uids = await inbox.SearchAsync(SearchQuery.All, cancellationToken);

            _logger.LogInformation("🔍 Found {Count} messages.", uids.Count);

            foreach (var uid in uids)
            {
                await ProcessMessage(uid, inbox, result, cancellationToken);
            }

            await inbox.ExpungeAsync(cancellationToken);

        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "❌ IMAP connection or processing failed.");
        }
        finally
        {
            if (client.IsConnected)
            {
                await client.DisconnectAsync(true, cancellationToken);
                _logger.LogInformation("🔌 Disconnected from IMAP server.");
            }
        }

        return result;

    }
    
    private async Task ProcessMessage(UniqueId uid, IMailFolder inbox, List<EmailMessage> result, CancellationToken cancellationToken)
    {
        try
        {
            var message = await inbox.GetMessageAsync(uid, cancellationToken);
            var attachments = message.Attachments.OfType<MimePart>().ToList();

            if (attachments.Count == 0) return;

            var allSucceeded = true;

            foreach (var attachment in attachments)
            {
                if (string.IsNullOrWhiteSpace(attachment.FileName))
                {
                    _logger.LogWarning("⚠️ Skipping unnamed attachment in UID {Uid}", uid);
                    continue;
                }

                try
                {
                    using var ms = new MemoryStream();
                    await attachment.Content.DecodeToAsync(ms, cancellationToken);

                    result.Add(new EmailMessage
                    {
                        Subject = message.Subject ?? "(No Subject)",
                        AttachmentName = attachment.FileName,
                        AttachmentBytes = ms.ToArray()
                    });
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "❌ Failed to process attachment {FileName} in UID {Uid}", attachment.FileName, uid);
                    allSucceeded = false;
                }
            }

            if (allSucceeded)
            {
                await inbox.AddFlagsAsync(uid, MessageFlags.Deleted, true, cancellationToken);
            }
            else
            {
                _logger.LogWarning("⚠️ UID {Uid} not deleted due to attachment errors", uid);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "❌ Failed to process message UID {Uid}", uid);
        }
    }
}
