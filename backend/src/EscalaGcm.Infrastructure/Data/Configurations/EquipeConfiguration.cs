using EscalaGcm.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EscalaGcm.Infrastructure.Data.Configurations;

public class EquipeConfiguration : IEntityTypeConfiguration<Equipe>
{
    public void Configure(EntityTypeBuilder<Equipe> builder)
    {
        builder.ToTable("equipes");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Nome).HasMaxLength(200).IsRequired();
        builder.HasIndex(x => x.Nome).IsUnique();
    }
}

public class EquipeMembroConfiguration : IEntityTypeConfiguration<EquipeMembro>
{
    public void Configure(EntityTypeBuilder<EquipeMembro> builder)
    {
        builder.ToTable("equipe_membros");
        builder.HasKey(x => x.Id);
        builder.HasIndex(x => new { x.EquipeId, x.GuardaId }).IsUnique();
        builder.HasOne(x => x.Equipe).WithMany(e => e.Membros).HasForeignKey(x => x.EquipeId);
        builder.HasOne(x => x.Guarda).WithMany(g => g.EquipeMembros).HasForeignKey(x => x.GuardaId);
    }
}
