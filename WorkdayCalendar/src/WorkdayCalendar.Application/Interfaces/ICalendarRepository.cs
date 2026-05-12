using WorkdayCalendar.Application.DTO;

namespace WorkdayCalendar.Application.Interfaces;

/// <summary>
/// Persistence contract for the calendar's configuration.
/// </summary>
public interface ICalendarRepository
{
    /// <summary>Returns the current calendar configuration.</summary>
    CalendarConfiguration Load();

    /// <summary>Update the calendar configuration.</summary>
    void Save(CalendarConfiguration configuration);
}