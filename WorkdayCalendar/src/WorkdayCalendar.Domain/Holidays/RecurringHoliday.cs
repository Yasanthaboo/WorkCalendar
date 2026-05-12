using WorkdayCalendar.Domain.Exceptions;

namespace WorkdayCalendar.Domain.Holidays;

/// <summary>
/// A holiday that recurs on the same month and day every year.
/// </summary>
public sealed class RecurringHoliday : Holiday
{
    public int Month { get; }
    public int Day { get; }

    public RecurringHoliday(int month, int day)
    {
        if (month < 1 || month > 12)
            throw new WorkdayCalendarException($"Month {month} is invalid. Month must be between 1 and 12.");
        // Feb 29 is valid — it only applies in leap years.
        if (!(month == 2 && day == 29) && (day < 1 || day > DateTime.DaysInMonth(2001, month)))
            throw new WorkdayCalendarException($"Day {day} is invalid for month {month}.");

        Month = month;
        Day = day;
    }

    public override bool OccursOn(DateOnly date)
    {
        if (date.Month != Month) return false;
        if (date.Day != Day) return false;
        return true;
    }

    public override bool Equals(Holiday? other) =>
        other is RecurringHoliday recurringHoliday && recurringHoliday.Month == Month && recurringHoliday.Day == Day;

    public override int GetHashCode() => HashCode.Combine(nameof(RecurringHoliday), Month, Day);

    public override string ToString() => $"Recurring Holiday: {Month:D2}-{Day:D2} every year";
}