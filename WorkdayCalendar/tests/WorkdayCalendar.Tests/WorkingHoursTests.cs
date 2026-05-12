using WorkdayCalendar.Domain.Exceptions;

namespace WorkdayCalendar.Tests;
public sealed class WorkingHoursTests
{
    [Fact]
    public void SetWorkdayStartAndStop_OvernightShift_IsValid()
    {
        // Arrange
        var calendar = new WorkdayCalendar.Domain.WorkdayCalendar();
        calendar.SetWorkdayStartAndStop(new TimeOnly(22, 0), new TimeOnly(6, 0));

        // Assert
        Assert.Equal(new TimeOnly(22, 0), calendar.WorkStart);
        Assert.Equal(new TimeOnly(6, 0), calendar.WorkEnd);
    }

    [Fact]
    public void SetWorkdayStartAndStop_EqualTimes_Throws()
    {
        var calendar = new WorkdayCalendar.Domain.WorkdayCalendar();
        Assert.Throws<WorkdayCalendarException>(() =>
            calendar.SetWorkdayStartAndStop(new TimeOnly(9, 0), new TimeOnly(9, 0)));
    }

    [Fact]
    public void SetWorkdayStartAndStop_NormalShift_IsValid()
    {
        // Arrange
        var calendar = new WorkdayCalendar.Domain.WorkdayCalendar();

        // Act
        calendar.SetWorkdayStartAndStop(new TimeOnly(9, 0), new TimeOnly(17, 0));

        // Assert
        Assert.Equal(new TimeOnly(9, 0), calendar.WorkStart);
        Assert.Equal(new TimeOnly(17, 0), calendar.WorkEnd);
    }
}