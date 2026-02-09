using EscalaGcm.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EscalaGcm.Infrastructure.Data.Configurations;

public class ViaturaConfiguration : IEntityTypeConfiguration<Viatura>
{
    public void Configure(EntityTypeBuilder<Viatura> builder)
    {
        builder.ToTable("viaturas");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Identificador).HasMaxLength(100).IsRequired();
        builder.HasIndex(x => x.Identificador).IsUnique();
    }
}
