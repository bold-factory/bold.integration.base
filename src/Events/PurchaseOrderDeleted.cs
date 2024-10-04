namespace Bold.Integration.Base.Events;

public record PurchaseOrderDeleted(string OrderId, string? ExternalReference);