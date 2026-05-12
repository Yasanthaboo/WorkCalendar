namespace WorkdayCalendar.Application.DTO;

/// <summary>
/// Represents state of a calendar instance.
/// </summary>
public sealed record CalendarConfiguration(
    TimeOnly WorkStart,
    TimeOnly WorkEnd,
    IReadOnlyList<HolidayDto> Holidays);