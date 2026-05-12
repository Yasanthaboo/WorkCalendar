namespace WorkdayCalendar.Domain.Holidays
{
    /// <summary>
    /// Abstract value object representing a holiday.
    /// </summary>
    public abstract class Holiday : IEquatable<Holiday>
    {
        public abstract bool OccursOn(DateOnly date);

        public abstract bool Equals(Holiday? other);

        public override bool Equals(object? obj) => obj is Holiday holiday && Equals(holiday);

        public abstract override int GetHashCode();
    }
}