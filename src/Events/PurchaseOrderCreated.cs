namespace Bold.Integration.Base.Events;

public record PurchaseOrderCreated(string OrderId, string? ExternalReference, string SupplierId, string Code, string Name, string? Notes);