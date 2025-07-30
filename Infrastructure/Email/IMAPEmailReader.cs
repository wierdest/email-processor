using Domain.Entities;
using Domain.Interfaces;
using Infrastructure.Email.Settings;
using MailKit;
using MailKit.Net.Imap;
using MailKit.Search;
using Microsoft.Extensions.Options;
using MimeKit;

namespace Infrastructure.Email;

public class IMAPEmailReader(IOptions<IMAPSettings> options) : IEmailReader
{
    private readonly IMAPSettings _settings = options.Value;
    public async Task<IEnumerable<EmailMessage>> ReadEmailsAsync(CancellationToken cancellationToken)
    {
        var result = new List<EmailMessage>();

        using var client = new ImapClient();
        await client.ConnectAsync(_settings.Host, _settings.Port, true, cancellationToken);
        await client.AuthenticateAsync(_settings.Username, _settings.Password, cancellationToken);

        var inbox = client.Inbox;
        await inbox.OpenAsync(FolderAccess.ReadWrite, cancellationToken);

        var uids = await inbox.SearchAsync(SearchQuery.NotSeen, cancellationToken);

        foreach (var uid in uids)
        {
            var message = await inbox.GetMessageAsync(uid, cancellationToken);

            foreach (var attachment in message.Attachments.OfType<MimePart>())
            {
                if (attachment is MimePart part)
                {
                    using var ms = new MemoryStream();
                    await part.Content.DecodeToAsync(ms, cancellationToken);

                    result.Add(new EmailMessage
                    {
                        Subject = message.Subject ?? "(No Subject)",
                        AttachmentName = part.FileName ?? "unknown",
                        AttachmentBytes = ms.ToArray()
                    });

                    await inbox.AddFlagsAsync(uid, MessageFlags.Deleted, true, cancellationToken);

                }
            }
        }
        await inbox.ExpungeAsync(cancellationToken);
        await client.DisconnectAsync(true, cancellationToken);
        return result;

    }
}
