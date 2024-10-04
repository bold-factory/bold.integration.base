namespace Bold.Integration.Base.Services;

public static class DateExtensions
{
    public static DateTimeOffset ToSpainOffset(this DateOnly date)
    {
        var dateTime = date.ToDateTime(TimeOnly.MinValue);
        var spainTimeZone = TimeZoneInfo.FindSystemTimeZoneById("Europe/Madrid");
        return new DateTimeOffset(dateTime: dateTime, offset: spainTimeZone.GetUtcOffset(dateTime));
    }
}