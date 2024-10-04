using Bold.Integration.Base.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Bold.Integration.Base.Persistence.Configurations;

public class SyncStateConfiguration : IEntityTypeConfiguration<SyncState>
{
    public void Configure(EntityTypeBuilder<SyncState> builder)
    {
        builder.ToTable("SyncStates");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Kind);
        builder.Property(x => x.LastProcessedChangeId);
        builder.Property(x => x.LastUpdated);

        builder.HasIndex(x => x.Kind);
    }
}