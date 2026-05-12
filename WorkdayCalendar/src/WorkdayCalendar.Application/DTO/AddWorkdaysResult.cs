namespace WorkdayCalendar.Application.DTO;

/// <summary>
/// Output from the AddWorkdays use case.
/// </summary>
public sealed record AddWorkdaysResult(
    DateTime ResultDatetime,
    string Summary);