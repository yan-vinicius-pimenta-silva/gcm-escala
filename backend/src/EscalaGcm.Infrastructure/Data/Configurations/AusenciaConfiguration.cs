using EscalaGcm.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EscalaGcm.Infrastructure.Data.Configurations;

public class AusenciaConfiguration : IEntityTypeConfiguration<Ausencia>
{
    public void Configure(EntityTypeBuilder<Ausencia> builder)
    {
        builder.ToTable("ausencias");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Motivo).HasConversion<string>().HasMaxLength(30);
        builder.Property(x => x.Observacoes).HasMaxLength(500);
        builder.HasIndex(x => new { x.GuardaId, x.DataInicio, x.DataFim });
        builder.HasOne(x => x.Guarda).WithMany(g => g.Ausencias).HasForeignKey(x => x.GuardaId);
    }
}
