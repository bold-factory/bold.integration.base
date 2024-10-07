using Bold.Integration.Base.Clients;
using Bold.Integration.Base.Entities;
using Bold.Integration.Base.Observability;

namespace Bold.Integration.Base.Services;

public class SkusService(BoldClient boldClient,
                          ErpClient erpClient,
                          SyncStateService syncStateService,
                          ErrorLoggerService errorLoggerService,
                          ILogger<SkusService> logger)
{
    public async Task SyncErpToBold(CancellationToken cancellationToken)
    {
        using var activity = Diagnostics.StartActivity("Syncing SKUs");
        var lastUpdateId = await syncStateService.GetLastProcessedIdAsync(kind: CollectionKind.Skus,
                                                                          cancellationToken: cancellationToken);
        var items = await erpClient.GetSkus(idChange: lastUpdateId, cancellationToken: cancellationToken);
        foreach (var sku in items)
        {
            using var skuActivity = Diagnostics.StartActivity($"Syncing SKU {sku.Code}");
            skuActivity?.AddTags(sku);
            try
            {
                await SyncSku(sku: sku, cancellationToken: cancellationToken);
            }
            catch (Exception e)
            {
                logger.LogError(exception: e, message: "Error al procesar el artículo {itemReference}", sku.Code);
                await errorLoggerService.RecordErrorAsync(kind: CollectionKind.Skus, entity: sku, error: e.ToString(),
                                                          cancellationToken: cancellationToken);
            }
            finally
            {
                await syncStateService.UpdateLastProcessedIdAsync(kind: CollectionKind.Skus,
                                                                  lastProcessedId: sku.ChangeId,
                                                                  cancellationToken: cancellationToken);
            }
        }
    }
    private async Task SyncSku(ErpClient.Sku sku, CancellationToken cancellationToken)
    {
        var boldSku = await GetSku(sku: sku, cancellationToken: cancellationToken);

        if (sku.Active && boldSku is null)
        {
            await CreateSku(sku: sku, cancellationToken: cancellationToken);
        }
        else if (sku.Active && boldSku is not null)
        {
            await UpdateSku(sku: sku, productReference: boldSku.ProductId, cancellationToken: cancellationToken);
        }
        else if (!sku.Active && boldSku is not null)
        {
            await DeleteSku(sku: sku, cancellationToken: cancellationToken);
        }
    }
    private async Task CreateSku(ErpClient.Sku sku, CancellationToken cancellationToken)
    {
        var isRawMaterial = sku.SupplierId == 0;

        await boldClient.Items_SKUs_CreateAsync(body: new CreateSkuRequest(code: sku.Code,
                                                                           description: sku.Description,
                                                                           externalReference: sku.Id.ToString(),
                                                                           manageLots: !isRawMaterial,
                                                                           name: sku.Name,
                                                                           productReference: null,
                                                                           replenishmentMode: isRawMaterial
                                                                               ? ReplenishmentMode.Batched
                                                                               : ReplenishmentMode.Direct,
                                                                           replenishmentSource: isRawMaterial
                                                                               ? ReplenishmentSource.Purchase
                                                                               : ReplenishmentSource.Manufacture,
                                                                           values: []),
                                                cancellationToken: cancellationToken);
    }
    private async Task UpdateSku(ErpClient.Sku sku, string? productReference, CancellationToken cancellationToken)
    {
        await boldClient.Items_SKUs_UpdateAsync(skuReference: sku.Id.ToString(),
                                                body: new UpdateSkuRequest(code: sku.Code,
                                                                           description: sku.Description,
                                                                           name: sku.Name,
                                                                           productReference: productReference),
                                                cancellationToken: cancellationToken);
    }
    private async Task DeleteSku(ErpClient.Sku sku, CancellationToken cancellationToken)
    {
        await boldClient.Items_SKUs_DeleteAsync(skuReference: sku.Id.ToString(), cancellationToken: cancellationToken);
    }
    private async Task<SkuResponse?> GetSku(ErpClient.Sku sku, CancellationToken cancellationToken)
    {
        try
        {
            return await boldClient.Items_SKUs_GetOneAsync(skuReference: sku.Id.ToString(), cancellationToken: cancellationToken);
        }
        catch (BoldApiException exception) when (exception.StatusCode == 404)
        {
            return null;
        }
    }
}