using EscalaGcm.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EscalaGcm.Infrastructure.Data.Configurations;

public class PosicaoConfiguration : IEntityTypeConfiguration<Posicao>
{
    public void Configure(EntityTypeBuilder<Posicao> builder)
    {
        builder.ToTable("posicoes");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Nome).HasMaxLength(200).IsRequired();
        builder.HasIndex(x => x.Nome).IsUnique();
    }
}
