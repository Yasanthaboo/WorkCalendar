using WorkdayCalendar.Application.DTO;
using WorkdayCalendar.Application.Interfaces;

namespace WorkdayCalendar.Infrastructure.Persistence;

/// <summary>
/// Stores the calendar configuration.
/// </summary>
public sealed class InMemoryCalendarRepository : ICalendarRepository
{
    private static readonly CalendarConfiguration DefaultConfig = new(
        WorkStart: new TimeOnly(8, 0),
        WorkEnd: new TimeOnly(16, 0),
        Holidays: []);

    private CalendarConfiguration _current = DefaultConfig;

    public CalendarConfiguration Load() => _current;

    public void Save(CalendarConfiguration configuration) =>
        _current = configuration;
}