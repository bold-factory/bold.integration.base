using Bold.Integration.Base.Clients;
using Bold.Integration.Base.Entities;
using Bold.Integration.Base.Events;

namespace Bold.Integration.Base.Endpoints;

public class PurchaseOrderEndpoints : IEndpoint
{
    public void Register(WebApplication app)
    {
        app.MapPost(pattern: "purchaseOrderCreated", handler: CreatePurchaseOrder);
    }
    private async Task CreatePurchaseOrder(Event<PurchaseOrderCreated> e,
                                           BoldClient boldClient,
                                           CancellationToken cancellationToken)
    {
        var payload = e.Payload;
        var supplier = await boldClient.Planning_Suppliers_GetOneAsync(supplierReference: payload.SupplierId, cancellationToken: cancellationToken);
        //do something here with the order
    }
}