using EscalaGcm.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EscalaGcm.Infrastructure.Data.Configurations;

public class HorarioConfiguration : IEntityTypeConfiguration<Horario>
{
    public void Configure(EntityTypeBuilder<Horario> builder)
    {
        builder.ToTable("horarios");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Inicio).IsRequired();
        builder.Property(x => x.Fim).IsRequired();
        builder.Property(x => x.Descricao).HasMaxLength(100);
        builder.HasIndex(x => new { x.Inicio, x.Fim }).IsUnique();
    }
}
