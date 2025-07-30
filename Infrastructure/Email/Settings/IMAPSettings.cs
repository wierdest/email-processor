using System;

namespace Infrastructure.Email.Settings;

public class IMAPSettings
{
    public string Host { get; init; } = string.Empty;
    public int Port { get; init; } = 993;
    public string Username { get; init; } = string.Empty;
    public string Password { get; init; } = string.Empty;
}
