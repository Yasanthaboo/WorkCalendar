using WorkdayCalendar.Domain.Holidays;

namespace WorkdayCalendar.Domain;

/// <summary>
/// Defines the contract for the Workday Calendar aggregate.
/// </summary>
public interface IWorkdayCalendar
{
    /// <summary>
    /// Sets the daily working window.
    /// </summary>
    void SetWorkdayStartAndStop(TimeOnly start, TimeOnly stop);

    /// <summary>
    /// Gets the current working day start time.
    /// </summary>
    TimeOnly WorkStart { get; }

    /// <summary>
    /// Gets the current working day end time.
    /// </summary>
    TimeOnly WorkEnd { get; }

    /// <summary>
    /// Registers a one-off holiday.
    /// </summary>
    void AddHoliday(DateOnly date);

    /// <summary>
    /// Registers a month/day combination as a recurring holiday.
    /// </summary>
    void AddRecurringHoliday(int month, int day);

    /// <summary>
    /// Removes a previously registered holiday.
    /// </summary>
    void RemoveHoliday(Holiday holiday);

    /// <summary>
    /// Returns all registered holidays.
    /// </summary>
    IReadOnlyCollection<Holiday> GetHolidays();

    /// <summary>
    /// Returns true when <paramref name="date"/> is a working day.
    /// </summary>
    bool IsWorkday(DateOnly date);

    /// <summary>
    /// Adds <paramref name="increment"/> fractional working days to
    /// <paramref name="startDate"/> and returns the resulting datetime.
    /// The result always falls within working hours on a working day.
    /// </summary>
    DateTime AddWorkdays(DateTime startDate, double increment);
}