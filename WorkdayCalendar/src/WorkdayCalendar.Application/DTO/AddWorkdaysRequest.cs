namespace WorkdayCalendar.Application.DTO;

/// <summary>
/// Input to the AddWorkdays use case.
/// </summary>
public sealed record AddWorkdaysRequest(
    DateTime StartDatetime,
    double Increment);