namespace WorkdayCalendar.Application.DTO;

/// <summary>
/// Data transfer object representing a holiday.
/// </summary>
public sealed record HolidayDto(
    HolidayType Type,
    DateOnly? Date,
    int? Month,
    int? Day,
    string DisplayName);