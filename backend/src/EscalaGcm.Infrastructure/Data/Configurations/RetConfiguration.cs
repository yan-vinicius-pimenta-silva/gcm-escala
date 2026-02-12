using EscalaGcm.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EscalaGcm.Infrastructure.Data.Configurations;

public class RetConfiguration : IEntityTypeConfiguration<Ret>
{
    public void Configure(EntityTypeBuilder<Ret> builder)
    {
        builder.ToTable("rets");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.TipoRet).HasConversion<string>().HasMaxLength(20);
        builder.Property(x => x.Observacao).HasMaxLength(500);
        builder.HasIndex(x => new { x.GuardaId, x.Data });
        builder.HasOne(x => x.Guarda).WithMany(g => g.Rets).HasForeignKey(x => x.GuardaId);
        builder.HasOne(x => x.Evento).WithMany(e => e.Rets).HasForeignKey(x => x.EventoId).IsRequired(false);
    }
}
