using WorkdayCalendar.Domain.Holidays;

namespace WorkdayCalendar.Tests;

public sealed class HolidayTests
{
    [Fact]
    public void SingleHoliday_OccursOnly_OnSpecificDate()
    {
        var holiday = new SingleHoliday(new DateOnly(2004, 5, 27));
        Assert.True(holiday.OccursOn(new DateOnly(2004, 5, 27)));
        Assert.False(holiday.OccursOn(new DateOnly(2004, 5, 28)));
        Assert.False(holiday.OccursOn(new DateOnly(2005, 5, 27)));
    }

    [Fact]
    public void RecurringHoliday_OccursEveryYear_OnSameMonthDay()
    {
        var holiday = new RecurringHoliday(5, 17);
        Assert.True(holiday.OccursOn(new DateOnly(2004, 5, 17)));
        Assert.True(holiday.OccursOn(new DateOnly(2020, 5, 17)));
        Assert.False(holiday.OccursOn(new DateOnly(2004, 5, 18)));
    }

    [Fact]
    public void SingleHoliday_ValueEquality_HoldsForSameDate()
    {
        var holiday1 = new SingleHoliday(new DateOnly(2024, 12, 25));
        var holiday2 = new SingleHoliday(new DateOnly(2024, 12, 25));
        Assert.Equal(holiday1, holiday2);
    }

    [Fact]
    public void RecurringHoliday_ValueEquality_HoldsForSameMonthDay()
    {
        var holiday1 = new RecurringHoliday(12, 25);
        var holiday2 = new RecurringHoliday(12, 25);
        Assert.Equal(holiday1, holiday2);
    }

    [Fact]
    public void AddHoliday_DuplicateDates_AreIgnored()
    {
        var calendar = new WorkdayCalendar.Domain.WorkdayCalendar();
        calendar.AddHoliday(new DateOnly(2024, 1, 1));
        calendar.AddHoliday(new DateOnly(2024, 1, 1));
        Assert.Single(calendar.GetHolidays());
    }

    [Fact]
    public void RemoveHoliday_RemovesCorrectly()
    {
        var calendar = new WorkdayCalendar.Domain.WorkdayCalendar();
        calendar.AddHoliday(new DateOnly(2024, 1, 1));
        calendar.RemoveHoliday(new SingleHoliday(new DateOnly(2024, 1, 1)));
        Assert.Empty(calendar.GetHolidays());
    }
}