using Microsoft.Extensions.DependencyInjection;
using WorkdayCalendar.Application.Interfaces;
using WorkdayCalendar.Infrastructure.Persistence;

namespace WorkdayCalendar.Infrastructure;

/// <summary>
/// Extension method to register Infrastructure layer services.
/// </summary>
public static class InfrastructureServiceExtensions
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services)
    {
        services.AddSingleton<ICalendarRepository, InMemoryCalendarRepository>();
        return services;
    }
}