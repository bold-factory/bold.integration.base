using Bold.Integration.Base.Entities;
using Microsoft.EntityFrameworkCore;

namespace Bold.Integration.Base.Persistence;

public class IntegrationDbContext(DbContextOptions options) : DbContext(options)
{
    public DbSet<SyncState> SyncStates => Set<SyncState>();
    public DbSet<EntityError> EntityErrors => Set<EntityError>();
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(IntegrationDbContext).Assembly);
    }
}