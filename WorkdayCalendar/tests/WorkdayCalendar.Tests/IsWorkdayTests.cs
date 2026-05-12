namespace WorkdayCalendar.Tests;

public sealed class IsWorkdayTests
{
    private readonly WorkdayCalendar.Domain.WorkdayCalendar _calendar = CalendarFactory.CreateWorkCalendar();

    [Theory]
    [InlineData(2004, 5, 24, true)]   // Monday – regular workday
    [InlineData(2004, 5, 17, false)]  // Monday but recurring holiday
    [InlineData(2004, 5, 27, false)]  // Thursday – single holiday
    [InlineData(2004, 5, 22, false)]  // Saturday
    [InlineData(2004, 5, 23, false)]  // Sunday
    [InlineData(2004, 5, 18, true)]   // Tuesday after recurring holiday
    public void IsWorkday_ReturnsExpected(int year, int month, int day, bool expected)
    {
        var date = new DateOnly(year, month, day);
        Assert.Equal(expected, _calendar.IsWorkday(date));
    }
}