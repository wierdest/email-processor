using Application.ProcessEmail;
using Microsoft.Extensions.DependencyInjection;

namespace Application.Extensions;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddTransient<IProcessEmailHandler, ProcessEmailHandler>();
        return services;
    }
}
