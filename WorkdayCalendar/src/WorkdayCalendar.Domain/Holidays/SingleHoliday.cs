namespace WorkdayCalendar.Domain.Holidays;

/// <summary>
/// A holiday occurs on specific calendar date.
/// </summary>
public sealed class SingleHoliday : Holiday
{
    public DateOnly Date { get; }

    public SingleHoliday(DateOnly date) => Date = date;

    public override bool OccursOn(DateOnly date) => Date == date;

    public override bool Equals(Holiday? other) =>
        other is SingleHoliday s && s.Date == Date;

    public override int GetHashCode() => HashCode.Combine(nameof(SingleHoliday), Date);

    public override string ToString() => $"Holiday: {Date:yyyy-MM-dd}";
}