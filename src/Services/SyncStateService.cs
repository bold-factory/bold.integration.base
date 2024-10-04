using Bold.Integration.Base.Entities;
using Bold.Integration.Base.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Bold.Integration.Base.Services;

public class SyncStateService(IntegrationDbContext dbContext)
{
    public async Task<long> GetLastProcessedIdAsync(CollectionKind kind, CancellationToken cancellationToken = default)
    {
        var state = await dbContext.SyncStates
                                   .FirstOrDefaultAsync(e => e.Kind == kind, cancellationToken: cancellationToken);

        return state?.LastProcessedChangeId ?? 0;
    }
    public async Task UpdateLastProcessedIdAsync(CollectionKind kind, long lastProcessedId, CancellationToken cancellationToken = default)
    {
        var state = await dbContext.SyncStates
                                   .FirstOrDefaultAsync(e => e.Kind == kind, cancellationToken: cancellationToken);

        if (state == null)
        {
            state = new SyncState
            {
                Kind = kind,
                LastProcessedChangeId = lastProcessedId,
                LastUpdated = DateTime.UtcNow
            };
            dbContext.SyncStates.Add(state);
        }
        else
        {
            state.LastProcessedChangeId = lastProcessedId;
            state.LastUpdated = DateTime.UtcNow;
        }

        await dbContext.SaveChangesAsync(cancellationToken);
    }
}