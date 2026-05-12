using Microsoft.Extensions.Logging.Abstractions;
using WorkdayCalendar.Application.DTO;
using WorkdayCalendar.Application.Interfaces;
using WorkdayCalendar.Application.Services;
using WorkdayCalendar.Domain.Exceptions;

namespace WorkdayCalendar.Tests;

/// <summary>
/// Unit tests for <see cref="WorkdayCalendarService"/>.
/// </summary>
public class ApplicationServiceTests
{
    private static WorkdayCalendarService CreateCalenderService(FakeRepository repository) =>
        new(repository, NullLogger<WorkdayCalendarService>.Instance);

    private sealed class FakeRepository : ICalendarRepository
    {
        private CalendarConfiguration _configuration = new(
            new TimeOnly(8, 0),
            new TimeOnly(16, 0),
            []);
        public CalendarConfiguration? LastSaved { get; private set; }

        public CalendarConfiguration Load() => _configuration;

        public void Save(CalendarConfiguration configuration)
        {
            _configuration = configuration;
            LastSaved = configuration;
        }

        public void Seed(CalendarConfiguration configuration) => _configuration = configuration;
    }

    [Fact]
    public void SetWorkdayHours_SavesNewHoursToRepository()
    {
        var repository = new FakeRepository();
        var calendarService = CreateCalenderService(repository);

        calendarService.SetWorkdayHours(new TimeOnly(9, 0), new TimeOnly(17, 0));

        Assert.NotNull(repository.LastSaved);
        Assert.Equal(new TimeOnly(9, 0), repository.LastSaved!.WorkStart);
        Assert.Equal(new TimeOnly(17, 0), repository.LastSaved!.WorkEnd);
    }

    [Fact]
    public void SetWorkdayHours_OvernightShift_SavesCorrectly()
    {
        var repository = new FakeRepository();
        var calendarService = CreateCalenderService(repository);

        // Act - Overnight shift should be valid
        calendarService.SetWorkdayHours(new TimeOnly(22, 0), new TimeOnly(6, 0));

        // Assert
        Assert.NotNull(repository.LastSaved);
        Assert.Equal(new TimeOnly(22, 0), repository.LastSaved!.WorkStart);
        Assert.Equal(new TimeOnly(6, 0), repository.LastSaved!.WorkEnd);
    }

    [Fact]
    public void SetWorkdayHours_EqualTimes_ThrowsAndDoesNotSave()
    {
        var repository = new FakeRepository();
        var calendarService = CreateCalenderService(repository);

        Assert.Throws<WorkdayCalendarException>(() =>
            calendarService.SetWorkdayHours(new TimeOnly(9, 0), new TimeOnly(9, 0)));

        Assert.Null(repository.LastSaved);
    }

    [Fact]
    public void AddSingleHoliday_AppearsInSavedHolidays()
    {
        var repository = new FakeRepository();
        var calendarService = CreateCalenderService(repository);
        var date = new DateOnly(2024, 12, 25);

        calendarService.AddSingleHoliday(date);

        var saved = Assert.Single(repository.LastSaved!.Holidays);
        Assert.Equal(HolidayType.Single, saved.Type);
        Assert.Equal(date, saved.Date);
    }

    [Fact]
    public void AddSingleHoliday_DoesNotChangeWorkingHours()
    {
        var repository = new FakeRepository();
        var calendarService = CreateCalenderService(repository);

        calendarService.AddSingleHoliday(new DateOnly(2024, 12, 25));

        Assert.Equal(new TimeOnly(8, 0), repository.LastSaved!.WorkStart);
        Assert.Equal(new TimeOnly(16, 0), repository.LastSaved!.WorkEnd);
    }

    [Fact]
    public void AddRecurringHoliday_AppearsInSavedHolidays()
    {
        var repository = new FakeRepository();
        var calendarService = CreateCalenderService(repository);

        calendarService.AddRecurringHoliday(5, 17);

        var saved = Assert.Single(repository.LastSaved!.Holidays);
        Assert.Equal(HolidayType.Recurring, saved.Type);
        Assert.Equal(5, saved.Month);
        Assert.Equal(17, saved.Day);
    }

    [Fact]
    public void AddRecurringHoliday_InvalidMonth_ThrowsAndDoesNotSave()
    {
        var repository = new FakeRepository();
        var calendarService = CreateCalenderService(repository);

        Assert.Throws<WorkdayCalendarException>(() => calendarService.AddRecurringHoliday(13, 1));

        Assert.Null(repository.LastSaved);
    }

    [Fact]
    public void AddRecurringHoliday_InvalidDay_ThrowsAndDoesNotSave()
    {
        var repository = new FakeRepository();
        var calendarService = CreateCalenderService(repository);

        Assert.Throws<WorkdayCalendarException>(() => calendarService.AddRecurringHoliday(2, 30));

        Assert.Null(repository.LastSaved);
    }

    [Fact]
    public void RemoveHoliday_SingleDto_RemovedFromSavedHolidays()
    {
        var date = new DateOnly(2024, 6, 1);
        var holiday = new HolidayDto(HolidayType.Single, date, null, null, "2024-06-01");
        var repository = new FakeRepository();
        repository.Seed(new(new TimeOnly(8, 0), new TimeOnly(16, 0), [holiday]));
        var calendarService = CreateCalenderService(repository);

        calendarService.RemoveHoliday(holiday);

        Assert.Empty(repository.LastSaved!.Holidays);
    }

    [Fact]
    public void RemoveHoliday_RecurringDto_RemovedFromSavedHolidays()
    {
        var recurringHoliday = new HolidayDto(HolidayType.Recurring, null, 5, 17, "Recurring Holiday: 05-17 every year");
        var repository = new FakeRepository();
        repository.Seed(new(new TimeOnly(8, 0), new TimeOnly(16, 0), [recurringHoliday]));
        var calendarService = CreateCalenderService(repository);

        calendarService.RemoveHoliday(recurringHoliday);

        Assert.Empty(repository.LastSaved!.Holidays);
    }

    [Fact]
    public void GetHolidays_ReturnsSavedHolidays()
    {
        var holiday = new HolidayDto(HolidayType.Single, new DateOnly(2004, 5, 27), null, null, "2004-05-27");
        var repository = new FakeRepository();
        repository.Seed(new(new TimeOnly(8, 0), new TimeOnly(16, 0), [holiday]));
        var calendarService = CreateCalenderService(repository);

        var result = calendarService.GetHolidays();

        var returned = Assert.Single(result);
        Assert.Equal(HolidayType.Single, returned.Type);
        Assert.Equal(new DateOnly(2004, 5, 27), returned.Date);
    }

    [Fact]
    public void GetHolidays_EmptyRepository_ReturnsEmpty()
    {
        var calendarService = CreateCalenderService(new FakeRepository());

        Assert.Empty(calendarService.GetHolidays());
    }

    [Fact]
    public void IsWorkday_Monday_ReturnsTrue()
    {
        var calendarService = CreateCalenderService(new FakeRepository());

        Assert.True(calendarService.IsWorkday(new DateOnly(2024, 5, 6)));
    }

    [Fact]
    public void IsWorkday_Saturday_ReturnsFalse()
    {
        var calendarService = CreateCalenderService(new FakeRepository());

        Assert.False(calendarService.IsWorkday(new DateOnly(2024, 5, 4)));
    }

    [Fact]
    public void IsWorkday_HolidayOnWeekday_ReturnsFalse()
    {
        var date = new DateOnly(2024, 5, 6); // Monday
        var holiday = new HolidayDto(HolidayType.Single, date, null, null, "2024-05-06");
        var repository = new FakeRepository();
        repository.Seed(new(new TimeOnly(8, 0), new TimeOnly(16, 0), [holiday]));
        var calendarService = CreateCalenderService(repository);

        Assert.False(calendarService.IsWorkday(date));
    }

    [Fact]
    public void IsWorkday_IsQueryOnly_DoesNotSaveToRepository()
    {
        var repository = new FakeRepository();
        var calendarService = CreateCalenderService(repository);

        calendarService.IsWorkday(new DateOnly(2024, 5, 6));

        Assert.Null(repository.LastSaved);
    }

    [Fact]
    public void AddWorkdays_ResultDatetimeIsCorrect()
    {
        var calendarService = CreateCalenderService(new FakeRepository());

        var result = calendarService.AddWorkdays(new AddWorkdaysRequest(
            new DateTime(2004, 5, 24, 8, 0, 0), 3.0));

        Assert.Equal(new DateTime(2004, 5, 27, 8, 0, 0), result.ResultDatetime);
    }

    [Fact]
    public void AddWorkdays_SummaryContainsStartIncrementAndResult()
    {
        var start = new DateTime(2004, 5, 24, 8, 0, 0);
        var calendarService = CreateCalenderService(new FakeRepository());

        var result = calendarService.AddWorkdays(new AddWorkdaysRequest(start, 3.0));

        Assert.Contains("2004-05-24 08:00", result.Summary);
        Assert.Contains("3", result.Summary);
        Assert.Contains("2004-05-27 08:00", result.Summary);
    }

    [Fact]
    public void AddWorkdays_IsQueryOnly_DoesNotSaveToRepository()
    {
        var repository = new FakeRepository();
        var calendarService = CreateCalenderService(repository);

        calendarService.AddWorkdays(new AddWorkdaysRequest(new DateTime(2004, 5, 24, 8, 0, 0), 1.0));

        Assert.Null(repository.LastSaved);
    }

    [Fact]
    public void AddWorkdays_HolidaysAreObserved()
    {
        var holidays = new HolidayDto[]
        {
            new(HolidayType.Recurring, null, 5, 17, "Recurring Holiday: 05-17 every year"),
            new(HolidayType.Single, new DateOnly(2004, 5, 27), null, null, "2004-05-27")
        };
        var repository = new FakeRepository();
        repository.Seed(new(new TimeOnly(8, 0), new TimeOnly(16, 0), holidays));
        var calendarService = CreateCalenderService(repository);

        var result = calendarService.AddWorkdays(new AddWorkdaysRequest(
            new DateTime(2004, 5, 24, 8, 0, 0), 3.0));

        Assert.Equal(new DateTime(2004, 5, 28, 8, 0, 0), result.ResultDatetime);
    }
}