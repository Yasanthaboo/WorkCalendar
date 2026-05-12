using Microsoft.Extensions.DependencyInjection;
using WorkdayCalendar.Application.Interfaces;
using WorkdayCalendar.Application.Services;

namespace WorkdayCalendar.Application;

/// <summary>
/// Extension method to register all Application layer services.
/// </summary>
public static class ApplicationServiceExtensions
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddScoped<IWorkdayCalendarService, WorkdayCalendarService>();
        return services;
    }
}