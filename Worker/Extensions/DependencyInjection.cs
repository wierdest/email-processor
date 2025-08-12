using System;
using Application.Extensions;
using Infrastructure.Extensions;
using TickerQ.DependencyInjection;

namespace Worker.Extensions;

public static class DependencyInjection
{
    public static IServiceCollection AddWorker(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddInfrastructure(configuration);
        services.AddApplication();
        
        services.AddTickerQ();

        return services;
    }
}