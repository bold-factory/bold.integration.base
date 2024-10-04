namespace Bold.Integration.Base.Events;

public record OrderFinished(string OrderId, string LotNumber, string? ExternalReference);