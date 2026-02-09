using EscalaGcm.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EscalaGcm.Infrastructure.Data.Configurations;

public class GuardaConfiguration : IEntityTypeConfiguration<Guarda>
{
    public void Configure(EntityTypeBuilder<Guarda> builder)
    {
        builder.ToTable("guardas");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Nome).HasMaxLength(200).IsRequired();
        builder.Property(x => x.Telefone).HasMaxLength(20);
        builder.HasIndex(x => x.Nome);
        builder.HasOne(x => x.Posicao).WithMany(p => p.Guardas).HasForeignKey(x => x.PosicaoId);
    }
}
