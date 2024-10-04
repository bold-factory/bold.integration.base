
namespace Bold.Integration.Base.Clients;

public class ErpClient(HttpClient client)
{
    public async Task<List<Sku>> GetSkus(long idChange, CancellationToken cancellationToken)
    {
        //just for ilustrative purporses, this could be a database read instead of an API call
        var response = await client.GetAsync(requestUri: $"items/before/{idChange}", cancellationToken: cancellationToken);
        response.EnsureSuccessStatusCode();
        var body = await response.Content.ReadFromJsonAsync<List<Sku>>(cancellationToken: cancellationToken);
        if (body is null) throw new InvalidCastException();
        return body;
    }
    public record Sku(long Id, long ChangeId, string Code, string Name, string Description, int SupplierId, bool Active);
    
}