using WorkdayCalendar.Domain.Exceptions;
using WorkdayCalendar.Domain.Holidays;

namespace WorkdayCalendar.Domain
{
    public sealed class WorkdayCalendar : IWorkdayCalendar
    {
        private TimeOnly _workStart = new(8, 0);
        private TimeOnly _workEnd = new(16, 0);
        private readonly HashSet<Holiday> _holidays = new();

        public void SetWorkdayStartAndStop(TimeOnly start, TimeOnly stop)
        {
            if (start == stop)
                throw new WorkdayCalendarException("Work start and stop cannot be the same time.");
            _workStart = start;
            _workEnd = stop;
        }

        public TimeOnly WorkStart => _workStart;

        public TimeOnly WorkEnd => _workEnd;

        public void AddHoliday(DateOnly date)
        {
            _holidays.Add(new SingleHoliday(date));
        }

        public void AddRecurringHoliday(int month, int day)
        {
            _holidays.Add(new RecurringHoliday(month, day));
        }

        public DateTime AddWorkdays(DateTime startDate, double increment)
        {
            var calculator = new WorkdayCalculator(this, _workStart, _workEnd);
            return calculator.AddWorkdays(startDate, increment);
        }

        public IReadOnlyCollection<Holiday> GetHolidays()
        {
            return _holidays.ToList().AsReadOnly();
        }

        public bool IsWorkday(DateOnly date)
        {
            if (date.DayOfWeek is DayOfWeek.Saturday or DayOfWeek.Sunday)
                return false;
            return !_holidays.Any(holiday => holiday.OccursOn(date));
        }

        public void RemoveHoliday(Holiday holiday)
        {
            _holidays.Remove(holiday);
        }
    }
}