namespace Bold.Integration.Base.Events;

public record OrderFinishReverted(string OrderId, string LotNumber, string? ExternalReference);