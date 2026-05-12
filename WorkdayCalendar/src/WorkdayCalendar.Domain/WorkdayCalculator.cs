namespace WorkdayCalendar.Domain;

/// <summary>
/// Domain service that encapsulates the fractional working-day arithmetic.
/// </summary>
internal sealed class WorkdayCalculator
{
    private readonly IWorkdayCalendar _calendar;
    private readonly TimeOnly _workStart;
    private readonly TimeOnly _workEnd;

    private bool CrossesMidnight => _workEnd < _workStart;

    private double HoursPerDay
    {
        get
        {
            var calculatedHours = (_workEnd.ToTimeSpan() - _workStart.ToTimeSpan()).TotalHours;
            return calculatedHours > 0 ? calculatedHours : calculatedHours + 24.0;
        }
    }

    private double ShiftElapsed(TimeOnly time)
    {
        var hours = (time.ToTimeSpan() - _workStart.ToTimeSpan()).TotalHours;
        return hours >= 0 ? hours : hours + 24.0;
    }

    private DateTime ToShiftDateTime(DateOnly shiftAnchorDate, double elapsed)
    {
        var totalHours = _workStart.ToTimeSpan().TotalHours + elapsed;
        var extraDays = (int)Math.Floor(totalHours / 24.0);
        var timeOfDay = totalHours - extraDays * 24.0;
        return shiftAnchorDate.AddDays(extraDays)
                              .ToDateTime(TimeOnly.FromTimeSpan(TimeSpan.FromHours(timeOfDay)));
    }

    public WorkdayCalculator(IWorkdayCalendar calendar, TimeOnly workStart, TimeOnly workEnd)
    {
        _calendar = calendar;
        _workStart = workStart;
        _workEnd = workEnd;
    }

    /// <summary>
    /// Adds <paramref name="increment"/> working days to
    /// <paramref name="startDate"/>.
    /// </summary>
    public DateTime AddWorkdays(DateTime startDate, double increment)
    {
        var normalised = Normalize(startDate);

        if (increment == 0d)
            return normalised;

        double offsetHours = increment * HoursPerDay;

        return offsetHours > 0
            ? AddForward(normalised, offsetHours)
            : AddBackward(normalised, Math.Abs(offsetHours));
    }

    /// <summary>
    /// Projects <paramref name="workDate"/> onto the working timeline:
    /// </summary>
    private DateTime Normalize(DateTime workDate)
    {
        var date = DateOnly.FromDateTime(workDate);
        var time = TimeOnly.FromDateTime(workDate);

        DateOnly effectiveDate;
        double elapsed;

        double elapsedHours = ShiftElapsed(time);

        if (elapsedHours < HoursPerDay)
        {
            if (CrossesMidnight && time < _workEnd)
                date = date.AddDays(-1);

            effectiveDate = date;
            elapsed = elapsedHours;
        }
        else if (!CrossesMidnight && time < _workStart)
        {
            effectiveDate = date;
            elapsed = 0;
        }
        else
        {
            effectiveDate = NextWorkday(date);
            elapsed = 0;
        }
        if (!_calendar.IsWorkday(effectiveDate))
        {
            effectiveDate = NextWorkday(effectiveDate);
            elapsed = 0;
        }

        return ToShiftDateTime(effectiveDate, elapsed);
    }

    private DateTime AddForward(DateTime normalised, double offsetHours)
    {
        var date = DateOnly.FromDateTime(normalised);
        var time = TimeOnly.FromDateTime(normalised);

        if (CrossesMidnight && time < _workEnd)
            date = date.AddDays(-1);

        double currentElapsed = ShiftElapsed(time);
        double remainingInShift = HoursPerDay - currentElapsed;

        while (offsetHours >= remainingInShift)
        {
            offsetHours -= remainingInShift;
            date = NextWorkday(date);
            remainingInShift = HoursPerDay;
            currentElapsed = 0;
        }

        return ToShiftDateTime(date, currentElapsed + offsetHours);
    }

    private DateTime AddBackward(DateTime normalised, double offsetHours)
    {
        var date = DateOnly.FromDateTime(normalised);
        var time = TimeOnly.FromDateTime(normalised);

        if (CrossesMidnight && time < _workEnd)
            date = date.AddDays(-1);

        double elapsedInShift = ShiftElapsed(time);

        while (offsetHours > elapsedInShift)
        {
            offsetHours -= elapsedInShift;
            date = PreviousWorkday(date);
            elapsedInShift = HoursPerDay;
        }

        return ToShiftDateTime(date, elapsedInShift - offsetHours);
    }

    private DateOnly NextWorkday(DateOnly from)
    {
        var next = from.AddDays(1);
        while (!_calendar.IsWorkday(next))
            next = next.AddDays(1);
        return next;
    }

    private DateOnly PreviousWorkday(DateOnly from)
    {
        var previous = from.AddDays(-1);
        while (!_calendar.IsWorkday(previous))
            previous = previous.AddDays(-1);
        return previous;
    }
}