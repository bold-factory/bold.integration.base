namespace Bold.Integration.Base.Entities;

public class SyncState
{
    public Guid Id { get; init; } = Guid.NewGuid();
    public CollectionKind Kind { get; init; }
    public long LastProcessedChangeId { get; set; }
    public DateTimeOffset LastUpdated { get; set; }
}