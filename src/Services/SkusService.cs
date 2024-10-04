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
        var skus = await erpClient.GetSkus(idChange: lastUpdateId, cancellationToken: cancellationToken);
        foreach (var item in skus)
        {
            using var skuActivity = Diagnostics.StartActivity($"Syncing SKU {item.Code}");
            skuActivity?.AddTags(item);
            if (item.Active)
            {
                await CreateOrUpdateItem(erpSku: item, cancellationToken: cancellationToken);
            }
            else
            {
                await TryDeleteItem(erpSku: item, cancellationToken: cancellationToken);
            }
            await syncStateService.UpdateLastProcessedIdAsync(kind: CollectionKind.Skus,
                                                              lastProcessedId: item.ChangeId,
                                                              cancellationToken: cancellationToken);
        }
    }
    private async Task CreateOrUpdateItem(ErpClient.Sku erpSku, CancellationToken cancellationToken)
    {
        try
        {
            var boldSku = await boldClient.SkusGET3Async(skuReference: erpSku.Id.ToString(), cancellationToken: cancellationToken);
            await boldClient.SkusPUTAsync(skuReference: erpSku.Id.ToString(), body: new UpdateSkuRequest
            {
                Name = erpSku.Name,
                Code = erpSku.Code,
                ProductReference = boldSku.ProductId,
            }, cancellationToken: cancellationToken);
        }
        catch (ApiException exception) when (exception.StatusCode == 404)
        {
            try
            {
                var isRawMaterial = erpSku.SupplierId == 0;
                await boldClient.SkusPOST2Async(body: new CreateSkuRequest
                {
                    ExternalReference = erpSku.Id.ToString(),
                    Name = erpSku.Name,
                    Code = erpSku.Code,
                    Description = erpSku.Description,
                    Values = [],
                    ProductReference = null,
                    ManageLots = !isRawMaterial,
                    ReplenishmentMode = isRawMaterial ? ReplenishmentMode.Batched : ReplenishmentMode.Direct,
                    ReplenishmentSource = isRawMaterial ? ReplenishmentSource.Purchase : ReplenishmentSource.Manufacture
                }, cancellationToken: cancellationToken);
            }
            catch (Exception e)
            {
                logger.LogError(exception: e, message: "Error al crear el artículo {itemReference}", args: erpSku.Code);
                await errorLoggerService.RecordErrorAsync(kind: CollectionKind.Skus, entity: erpSku, error: e.ToString(),
                                                          cancellationToken: cancellationToken);
            }
        }
        catch (Exception e)
        {
            logger.LogError(exception: e, message: "Error al actualizar el artículo {itemReference}", args: erpSku.Code);
            await errorLoggerService.RecordErrorAsync(kind: CollectionKind.Skus, entity: erpSku, error: e.ToString(),
                                                      cancellationToken: cancellationToken);
        }
    }
    private async Task TryDeleteItem(ErpClient.Sku erpSku, CancellationToken cancellationToken)
    {
        try
        {
            await boldClient.SkusGET3Async(skuReference: erpSku.Id.ToString(), cancellationToken: cancellationToken);
            await boldClient.SkusDELETEAsync(skuReference: erpSku.Id.ToString(), cancellationToken: cancellationToken);
        }
        catch (ApiException exception) when (exception.StatusCode == 404)
        {
            logger.LogInformation(message: "Skipping deletion of sku {skuReference} because it does not exist in Bold",
                                  args: erpSku.Code);
        }
        catch (Exception e)
        {
            logger.LogError(exception: e, message: "Error al eliminar el artículo {skuReference}", args: erpSku.Code);
            await errorLoggerService.RecordErrorAsync(kind: CollectionKind.Skus, entity: erpSku, error: e.ToString(),
                                                      cancellationToken: cancellationToken);
        }
    }
}