using Microsoft.Extensions.Logging;
using WorkdayCalendar.Application.DTO;
using WorkdayCalendar.Application.Interfaces;
using WorkdayCalendar.Domain.Exceptions;
using WorkdayCalendar.Domain.Holidays;

namespace WorkdayCalendar.Application.Services;

/// <summary>
/// Manages domain operations and maps between domain objects and DTOs.
/// </summary>
public sealed class WorkdayCalendarService : IWorkdayCalendarService
{
    private readonly ICalendarRepository _repository;
    private readonly ILogger<WorkdayCalendarService> _logger;

    public WorkdayCalendarService(ICalendarRepository repository, ILogger<WorkdayCalendarService> logger)
    {
        _repository = repository;
        _logger = logger;
    }

    public void SetWorkdayHours(TimeOnly start, TimeOnly end)
    {
        try
        {
            var calendar = PopulateCalendar();
            calendar.SetWorkdayStartAndStop(start, end);
            UpdateCalendar(calendar);
            _logger.LogInformation("Working hours set to {Start}–{End}", start, end);
        }
        catch (WorkdayCalendarException exception)
        {
            _logger.LogWarning(exception, "Invalid working hours: {0}–{1}", start, end);
            throw;
        }
    }

    public void AddSingleHoliday(DateOnly date)
    {
        var calendar = PopulateCalendar();
        calendar.AddHoliday(date);
        UpdateCalendar(calendar);
        _logger.LogInformation("Single holiday added: {0}", date);
    }

    public void AddRecurringHoliday(int month, int day)
    {
        try
        {
            var calendar = PopulateCalendar();
            calendar.AddRecurringHoliday(month, day);
            UpdateCalendar(calendar);
            _logger.LogInformation("Recurring holiday added: {Month:0}-{Day:1}", month, day);
        }
        catch (WorkdayCalendarException exception)
        {
            _logger.LogWarning(exception, "Invalid recurring holiday: month={0} day={1}", month, day);
            throw;
        }
    }

    public void RemoveHoliday(HolidayDto holiday)
    {
        var calendar = PopulateCalendar();
        calendar.RemoveHoliday(MapToDomain(holiday));
        UpdateCalendar(calendar);
        _logger.LogInformation("Holiday removed: {0}", holiday.DisplayName);
    }

    public IReadOnlyList<HolidayDto> GetHolidays() =>
        _repository.Load().Holidays;

    public bool IsWorkday(DateOnly date) =>
        PopulateCalendar().IsWorkday(date);

    public AddWorkdaysResult AddWorkdays(AddWorkdaysRequest request)
    {
        try
        {
            var result = PopulateCalendar().AddWorkdays(request.StartDatetime, request.Increment);
            var summary = $"{request.StartDatetime:yyyy-MM-dd HH:mm} + ({request.Increment:G6} wd) = {result:yyyy-MM-dd HH:mm}";
            _logger.LogInformation("AddWorkdays: {Summary}", summary);
            return new AddWorkdaysResult(result, summary);
        }
        catch (Exception exception)
        {
            _logger.LogError(exception, "AddWorkdays failed: start={0} increment={1}",
                request.StartDatetime, request.Increment);
            throw;
        }
    }

    /// <summary>
    /// Creates a <see cref="Domain.WorkdayCalendar"/> populated from the
    /// current repository state.
    /// </summary>
    private Domain.WorkdayCalendar PopulateCalendar()
    {
        var configuration = _repository.Load();
        var calendar = new Domain.WorkdayCalendar();
        calendar.SetWorkdayStartAndStop(configuration.WorkStart, configuration.WorkEnd);
        foreach (var holiday in configuration.Holidays)
            ApplyHolidayToCalendar(calendar, holiday);
        return calendar;
    }

    /// <summary>
    ///Updates <see cref="CalendarConfiguration"/> via the repository.
    /// </summary>
    private void UpdateCalendar(Domain.WorkdayCalendar calendar)
    {
        var configuration = new CalendarConfiguration(
            calendar.WorkStart,
            calendar.WorkEnd,
            calendar.GetHolidays().Select(MapToDto).ToList().AsReadOnly());
        _repository.Save(configuration);
    }

    private static void ApplyHolidayToCalendar(Domain.WorkdayCalendar calendar, HolidayDto holidayDto)
    {
        if (holidayDto.Type == HolidayType.Single)
        {
            calendar.AddHoliday(holidayDto.Date!.Value);
        }
        else
        {
            calendar.AddRecurringHoliday(holidayDto.Month!.Value, holidayDto.Day!.Value);
        }
    }

    private static HolidayDto MapToDto(Holiday holiday) => holiday switch
    {
        SingleHoliday singleHoliday => new HolidayDto(HolidayType.Single, singleHoliday.Date, null, null, singleHoliday.ToString()!),
        RecurringHoliday recurringHoliday => new HolidayDto(HolidayType.Recurring, null, recurringHoliday.Month, recurringHoliday.Day, recurringHoliday.ToString()!),
        _ => throw new InvalidOperationException($"Unknown holiday type: {holiday.GetType().Name}")
    };

    private static Holiday MapToDomain(HolidayDto holidayDto) => holidayDto.Type switch
    {
        HolidayType.Single => new SingleHoliday(holidayDto.Date!.Value),
        HolidayType.Recurring => new RecurringHoliday(holidayDto.Month!.Value, holidayDto.Day!.Value),
        _ => throw new InvalidOperationException($"Unknown HolidayType: {holidayDto.Type}")
    };
}