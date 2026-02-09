using EscalaGcm.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EscalaGcm.Infrastructure.Data.Configurations;

public class FeriasConfiguration : IEntityTypeConfiguration<Ferias>
{
    public void Configure(EntityTypeBuilder<Ferias> builder)
    {
        builder.ToTable("ferias");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Observacao).HasMaxLength(500);
        builder.HasIndex(x => new { x.GuardaId, x.DataInicio, x.DataFim });
        builder.HasOne(x => x.Guarda).WithMany(g => g.Ferias).HasForeignKey(x => x.GuardaId);
    }
}
