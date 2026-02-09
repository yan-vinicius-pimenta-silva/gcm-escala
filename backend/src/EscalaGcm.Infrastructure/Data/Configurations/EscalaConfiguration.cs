using EscalaGcm.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EscalaGcm.Infrastructure.Data.Configurations;

public class EscalaConfiguration : IEntityTypeConfiguration<Escala>
{
    public void Configure(EntityTypeBuilder<Escala> builder)
    {
        builder.ToTable("escalas");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Status).HasConversion<string>().HasMaxLength(20);
        builder.HasIndex(x => new { x.Ano, x.Mes, x.Quinzena, x.SetorId }).IsUnique();
        builder.HasIndex(x => new { x.SetorId, x.Ano, x.Mes, x.Quinzena });
        builder.HasOne(x => x.Setor).WithMany(s => s.Escalas).HasForeignKey(x => x.SetorId);
    }
}

public class EscalaItemConfiguration : IEntityTypeConfiguration<EscalaItem>
{
    public void Configure(EntityTypeBuilder<EscalaItem> builder)
    {
        builder.ToTable("escala_itens");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Observacao).HasMaxLength(500);
        builder.HasIndex(x => new { x.EscalaId, x.Data, x.TurnoId, x.HorarioId }).IsUnique();
        builder.HasIndex(x => new { x.EscalaId, x.Data });
        builder.HasOne(x => x.Escala).WithMany(e => e.Itens).HasForeignKey(x => x.EscalaId);
        builder.HasOne(x => x.Turno).WithMany(t => t.EscalaItens).HasForeignKey(x => x.TurnoId);
        builder.HasOne(x => x.Horario).WithMany(h => h.EscalaItens).HasForeignKey(x => x.HorarioId);
    }
}

public class EscalaAlocacaoConfiguration : IEntityTypeConfiguration<EscalaAlocacao>
{
    public void Configure(EntityTypeBuilder<EscalaAlocacao> builder)
    {
        builder.ToTable("escala_alocacoes");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Funcao).HasConversion<string>().HasMaxLength(20);
        builder.HasIndex(x => new { x.GuardaId, x.EscalaItemId });
        builder.HasIndex(x => new { x.EquipeId, x.EscalaItemId });
        builder.HasOne(x => x.EscalaItem).WithMany(ei => ei.Alocacoes).HasForeignKey(x => x.EscalaItemId);
        builder.HasOne(x => x.Guarda).WithMany(g => g.Alocacoes).HasForeignKey(x => x.GuardaId);
        builder.HasOne(x => x.Equipe).WithMany(e => e.Alocacoes).HasForeignKey(x => x.EquipeId);
        builder.HasOne(x => x.Viatura).WithMany(v => v.Alocacoes).HasForeignKey(x => x.ViaturaId);
    }
}
