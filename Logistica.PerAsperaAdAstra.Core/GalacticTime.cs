namespace LogisticaPerAsperaAdAstra.Core;

/// <summary>
/// Represents a date and time for simulations using a simplified 360-day calendar.
/// Each year has 12 months, and each month has 30 days. No leap years.
/// Stores time as the total number of minutes from an epoch (Year 1, Day 1).
/// </summary>
public readonly record struct GalacticDateTime(long TotalMinutes) : IComparable<GalacticDateTime>
{
    // Constants for the simplified calendar
    private const int MinutesInHour = 60;
    private const int HoursInDay = 24;
    private const int DaysInMonth = 30;
    private const int MonthsInYear = 12;

    private const int MinutesInDay = HoursInDay * MinutesInHour;
    private const int MinutesInMonth = DaysInMonth * MinutesInDay;
    private const int MinutesInYear = MonthsInYear * MinutesInMonth; // 360 days

    public long Year => TotalMinutes / MinutesInYear + 1;
    public int Month => (int)((TotalMinutes % MinutesInYear) / MinutesInMonth) + 1;
    public int Day => (int)((TotalMinutes % MinutesInYear % MinutesInMonth) / MinutesInDay) + 1;
    public int Hour => (int)(TotalMinutes % MinutesInDay / MinutesInHour);
    public int Minute => (int)TotalMinutes % MinutesInHour;

    public static GalacticDateTime From(long year, int month, int day, int hour, int minute)
    {
        long totalDays = (year - 1) * (MonthsInYear * DaysInMonth)
                         + (month - 1) * DaysInMonth
                         + (day - 1);

        return new GalacticDateTime(totalDays * MinutesInDay
                        + hour * MinutesInHour
                        + minute);
    }

    public GalacticDateTime AddMinutes(long minutes) => new GalacticDateTime(TotalMinutes + minutes);
    public GalacticDateTime AddHours(long hours) => new GalacticDateTime(TotalMinutes + (hours * MinutesInHour));
    public GalacticDateTime AddDays(long days) => new GalacticDateTime(TotalMinutes + (days * MinutesInDay));
    public GalacticDateTime AddMonths(long months) => new GalacticDateTime(TotalMinutes + (months * MinutesInMonth));
    public GalacticDateTime AddYears(long years) => new GalacticDateTime(TotalMinutes + (years * MinutesInYear));

    public override string ToString() => $"{Year:0000}-{Month:00}-{Day:00} {Hour:00}:{Minute:00}";

    // Comparison and Equality logic
    public int CompareTo(GalacticDateTime other) => TotalMinutes.CompareTo(other.TotalMinutes);

    public static bool operator <(GalacticDateTime left, GalacticDateTime right) => left.CompareTo(right) < 0;
    public static bool operator >(GalacticDateTime left, GalacticDateTime right) => left.CompareTo(right) > 0;
    public static bool operator <=(GalacticDateTime left, GalacticDateTime right) => left.CompareTo(right) <= 0;
    public static bool operator >=(GalacticDateTime left, GalacticDateTime right) => left.CompareTo(right) >= 0;

    // Increment, Decrement, and TimeSpan operators
    public static GalacticDateTime operator ++(GalacticDateTime dt) => dt.AddMinutes(1);
    public static GalacticDateTime operator --(GalacticDateTime dt) => dt.AddMinutes(-1);
    public static GalacticDateTime operator +(GalacticDateTime dt, TimeSpan ts) => dt.AddMinutes((long)ts.TotalMinutes);
    public static GalacticDateTime operator -(GalacticDateTime dt, TimeSpan ts) => dt.AddMinutes(-(long)ts.TotalMinutes);
    public static TimeSpan operator -(GalacticDateTime dt1, GalacticDateTime dt2) => TimeSpan.FromMinutes(dt1.TotalMinutes - dt2.TotalMinutes);
}