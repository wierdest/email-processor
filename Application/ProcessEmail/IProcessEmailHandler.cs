using System;

namespace Application.ProcessEmail;

public interface IProcessEmailHandler
{
    Task Handle(ProcessEmailCommand command, CancellationToken cancellationToken);
}
