using Bold.Integration.Base.Clients;

namespace Bold.Integration.Base.Events;

public record SkuStockLevelChanged(string SkuId,
                                   string LotNumber,
                                   string LocationId,
                                   string? Comment,
                                   StockChangeType Type,
                                   decimal Change,
                                   decimal CurrentStock);