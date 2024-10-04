namespace Bold.Integration.Base.Events;

public record PurchaseOrderLineAdded(string OrderId,
                                     string? OrderExternalReference,
                                     string LineId,
                                     string? LineExternalReference,
                                     int LineNumber,
                                     string SkuId,
                                     decimal Quantity);