namespace WorkdayCalendar.Tests;

/// <summary>
/// Tests AddWorkdays scenarios with non-standard working hours.
/// </summary>
public sealed class AddWorkdaysAlternativeHoursTests
{
    [Theory]
    [InlineData("09:00", "17:00", "2004-05-24 14:00", 0.5, "2004-05-25 10:00")] // Overflow to next day
    [InlineData("06:00", "14:00", "2004-05-24 06:00", 1.0, "2004-05-25 06:00")] // Early shift
    [InlineData("22:00", "06:00", "2004-05-24 22:00", 0.5, "2004-05-25 02:00")] // Night shift
    public void AddWorkdays_CustomWorkingHours_CalculatesCorrectly(
        string workStart, string workEnd, string start, double increment, string expected)
    {
        // Arrange
        var calendar = new Domain.WorkdayCalendar();
        calendar.SetWorkdayStartAndStop(
            TimeOnly.Parse(workStart),
            TimeOnly.Parse(workEnd));

        var startDt = DateTime.Parse(start);
        var expectedDt = DateTime.Parse(expected);

        // Act
        var result = calendar.AddWorkdays(startDt, increment);

        // Assert
        Assert.True(
            Math.Abs((result - expectedDt).TotalMinutes) < 1,
            $"Expected {expectedDt:yyyy-MM-dd HH:mm} but got {result:yyyy-MM-dd HH:mm}");
    }
}