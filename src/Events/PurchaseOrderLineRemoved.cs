namespace Bold.Integration.Base.Events;

public record PurchaseOrderLineRemoved(string OrderId,
                                       string? OrderExternalReference,
                                       string LineId,
                                       string? LineExternalReference);