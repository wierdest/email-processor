using Domain.Interfaces;
using Infrastructure.Email;
using Infrastructure.Email.Settings;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure.Extensions;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<IMAPSettings>(configuration.GetSection("IMAPSettings"));
        services.AddSingleton<IEmailReader, IMAPEmailReader>();
        return services;
    }
}
