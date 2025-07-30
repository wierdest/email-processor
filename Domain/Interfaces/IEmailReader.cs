using System;
using Domain.Entities;

namespace Domain.Interfaces;

public interface IEmailReader
{
    Task<IEnumerable<EmailMessage>> ReadEmailsAsync(CancellationToken cancellationToken);
}
