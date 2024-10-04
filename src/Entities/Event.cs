namespace Bold.Integration.Base.Entities;

public class Event<T> where T : class
{
    public Guid EventId { get; set; }
    public string EventName { get; init; } = string.Empty;
    public string Entity { get; init; } = string.Empty;
    public string EntityId { get; init; } = string.Empty;
    public DateTimeOffset OccurredOn { get; init; }
    public T Payload { get; init; } = null!;
    public int MajorVersion { get; init; }
    public int MinorVersion { get; init; }
}