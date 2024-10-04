using System.Text.Json;
using Bold.Integration.Base.Entities;
using Bold.Integration.Base.Persistence;

namespace Bold.Integration.Base.Services;

public class ErrorLoggerService(IntegrationDbContext dbContext)
{
    public async Task RecordErrorAsync<T>(CollectionKind kind, T entity, string error, CancellationToken cancellationToken = default)
    {
        var entityError = new EntityError
        {
            Error = error,
            Kind = kind,
            Content = JsonSerializer.Serialize(entity)
        };
        dbContext.EntityErrors.Add(entityError);

        await dbContext.SaveChangesAsync(cancellationToken);
    }
}