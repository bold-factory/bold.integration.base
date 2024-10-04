namespace Bold.Integration.Base.Events;

public record PurchaseOrderLineQuantityChanged(string OrderId,
                                               string? OrderExternalReference,
                                               string LineId,
                                               string? LineExternalReference,
                                               decimal Quantity);