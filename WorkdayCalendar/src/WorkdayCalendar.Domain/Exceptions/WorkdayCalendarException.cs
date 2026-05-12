namespace WorkdayCalendar.Domain.Exceptions
{
    /// <summary>
    /// Base exception for domain rule violations in the Workday Calendar.
    /// </summary>
    public class WorkdayCalendarException : Exception
    {
        public WorkdayCalendarException(string message) : base(message)
        {
        }

        public WorkdayCalendarException(string message, Exception inner) : base(message, inner)
        {
        }
    }
}