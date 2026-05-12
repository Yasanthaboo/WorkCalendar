namespace WorkdayCalendar.Tests;

/// <summary>
/// Helper that builds a <see cref="Domain.WorkdayCalendar"/> pre-configured
/// </summary>
internal static class CalendarFactory
{
    public static Domain.WorkdayCalendar CreateWorkCalendar()
    {
        var calendar = new Domain.WorkdayCalendar();
        calendar.SetWorkdayStartAndStop(new TimeOnly(8, 0), new TimeOnly(16, 0));
        calendar.AddRecurringHoliday(5, 17);
        calendar.AddHoliday(new DateOnly(2004, 5, 27));
        return calendar;
    }
}