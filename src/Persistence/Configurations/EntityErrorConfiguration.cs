using Bold.Integration.Base.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Bold.Integration.Base.Persistence.Configurations;

public class EntityErrorConfiguration : IEntityTypeConfiguration<EntityError>
{
    public void Configure(EntityTypeBuilder<EntityError> builder)
    {
        builder.ToTable("EntityErrors");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Kind);
        builder.Property(x => x.Content);
        builder.Property(x => x.Error);
    }
}