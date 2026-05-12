using WorkdayCalendar.Application.DTO;

namespace WorkdayCalendar.Application.Interfaces;

/// <summary>
/// Application-layer contract for the Workday Calendar feature.
/// </summary>
public interface IWorkdayCalendarService
{
    /// <summary>Sets the daily working window.</summary>
    void SetWorkdayHours(TimeOnly start, TimeOnly end);

    /// <summary>Registers a specific calendar date as a holiday.</summary>
    void AddSingleHoliday(DateOnly date);

    /// <summary>Registers a month+day combination as a yearly recurring holiday.</summary>
    void AddRecurringHoliday(int month, int day);

    /// <summary>Removes a previously registered holiday.</summary>
    void RemoveHoliday(HolidayDto holiday);

    /// <summary>Returns all registered holidays as DTOs.</summary>
    IReadOnlyList<HolidayDto> GetHolidays();

    /// <summary>Returns true when <paramref name="date"/> is a working day.</summary>
    bool IsWorkday(DateOnly date);

    /// <summary>
    /// Adds fractional working days to a start datetime and returns the result.
    /// </summary>
    AddWorkdaysResult AddWorkdays(AddWorkdaysRequest request);
}