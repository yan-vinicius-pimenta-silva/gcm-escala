using EscalaGcm.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EscalaGcm.Infrastructure.Data.Configurations;

public class UsuarioConfiguration : IEntityTypeConfiguration<Usuario>
{
    public void Configure(EntityTypeBuilder<Usuario> builder)
    {
        builder.ToTable("usuarios");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.NomeUsuario).HasMaxLength(100).IsRequired();
        builder.HasIndex(x => x.NomeUsuario).IsUnique();
        builder.Property(x => x.SenhaHash).IsRequired();
        builder.Property(x => x.NomeCompleto).HasMaxLength(200).IsRequired();
        builder.Property(x => x.Perfil).HasConversion<string>().HasMaxLength(20);
    }
}
