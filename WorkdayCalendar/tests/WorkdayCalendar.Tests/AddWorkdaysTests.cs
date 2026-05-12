namespace WorkdayCalendar.Tests;

/// <summary>
/// Parameterised tests covering all given test scenarios.
/// </summary>
public sealed class AddWorkdaysTests
{
    private readonly Domain.WorkdayCalendar _calendar = CalendarFactory.CreateWorkCalendar();

    [Theory]
    [InlineData("2004-05-24 18:05", -5.5, "2004-05-14 12:00")]
    [InlineData("2004-05-24 19:03", 44.723656, "2004-07-27 13:47")]
    [InlineData("2004-05-24 18:03", -6.7470217, "2004-05-13 10:02")]
    [InlineData("2004-05-24 08:03", 12.782709, "2004-06-10 14:18")]
    [InlineData("2004-05-24 07:03", 8.276628, "2004-06-04 10:12")]
    public void AddWorkdays_SrsExamples_ReturnExpected(
        string start, double increment, string expected)
    {
        var startDate = DateTime.Parse(start);
        var expectedDate = DateTime.Parse(expected);

        var result = _calendar.AddWorkdays(startDate, increment);

        Assert.True(
            Math.Abs((result - expectedDate).TotalMinutes) < 1,
            $"Expected {expectedDate:yyyy-MM-dd HH:mm} but got {result:yyyy-MM-dd HH:mm}");
    }

    [Fact]
    public void AddWorkdays_ZeroIncrement_AtWorkStart_ReturnsSameTime()
    {
        var start = new DateTime(2004, 5, 24, 8, 0, 0);
        Assert.Equal(start, _calendar.AddWorkdays(start, 0));
    }

    [Fact]
    public void AddWorkdays_ZeroIncrement_AtWorkEnd_ReturnsNextWorkdayStart()
    {
        var start = new DateTime(2004, 5, 24, 16, 0, 0);
        var expected = new DateTime(2004, 5, 25, 8, 0, 0);
        Assert.Equal(expected, _calendar.AddWorkdays(start, 0));
    }

    [Fact]
    public void AddWorkdays_QuarterDay_Forward_MidDay()
    {
        var start = new DateTime(2004, 5, 18, 8, 0, 0);
        var expected = new DateTime(2004, 5, 18, 10, 0, 0);
        Assert.Equal(expected, _calendar.AddWorkdays(start, 0.25));
    }

    [Fact]
    public void AddWorkdays_StartOnRecurringHoliday_AdvancesToNextWorkday()
    {
        var start = new DateTime(2004, 5, 17, 8, 0, 0);
        var expected = new DateTime(2004, 5, 18, 8, 0, 0);
        Assert.Equal(expected, _calendar.AddWorkdays(start, 0));
    }

    [Fact]
    public void AddWorkdays_StartOnSingleHoliday_AdvancesToNextWorkday()
    {
        var start = new DateTime(2004, 5, 27, 12, 0, 0);
        var expected = new DateTime(2004, 5, 28, 8, 0, 0);
        Assert.Equal(expected, _calendar.AddWorkdays(start, 0));
    }

    [Fact]
    public void AddWorkdays_OneFullDay_SkipsWeekend()
    {
        var start = new DateTime(2004, 5, 21, 8, 0, 0);
        var expected = new DateTime(2004, 5, 24, 8, 0, 0);
        Assert.Equal(expected, _calendar.AddWorkdays(start, 1.0));
    }

    [Fact]
    public void AddWorkdays_OneFullDay_SkipsRecurringHoliday()
    {
        var start = new DateTime(2004, 5, 17, 8, 0, 0);
        var expected = new DateTime(2004, 5, 19, 8, 0, 0);
        Assert.Equal(expected, _calendar.AddWorkdays(start, 1.0));
    }

    [Fact]
    public void AddWorkdays_QuarterDay_From1507_OverflowsToNextDay()
    {
        var start = new DateTime(2004, 5, 28, 15, 7, 0);
        var expected = new DateTime(2004, 5, 31, 9, 7, 0);
        Assert.Equal(expected, _calendar.AddWorkdays(start, 0.25));
    }

    [Fact]
    public void AddWorkdays_HalfDay_From0400_NormalisesBeforeAdding()
    {
        var start = new DateTime(2004, 5, 28, 4, 0, 0);
        var expected = new DateTime(2004, 5, 28, 12, 0, 0);
        Assert.Equal(expected, _calendar.AddWorkdays(start, 0.5));
    }
}