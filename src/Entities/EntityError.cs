namespace Bold.Integration.Base.Entities;

public class EntityError
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public CollectionKind Kind { get; init; }
    public string Content { get; init; } = string.Empty;
    public string Error { get; init; } = string.Empty;
    public DateTimeOffset OccurredOn { get; init; } = DateTimeOffset.UtcNow;
}