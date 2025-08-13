using System;
using Domain.DTOs;
using Domain.Entities;

namespace Domain.Interfaces;

public interface IEmailReader
{
    IAsyncEnumerable<ProcessedEmailMessage> ReadEmailsAsync(CancellationToken cancellationToken);
}
