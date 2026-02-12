using EscalaGcm.Domain.Common;
using EscalaGcm.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace EscalaGcm.Infrastructure.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<Usuario> Usuarios => Set<Usuario>();
    public DbSet<Setor> Setores => Set<Setor>();
    public DbSet<Posicao> Posicoes => Set<Posicao>();
    public DbSet<Turno> Turnos => Set<Turno>();
    public DbSet<Horario> Horarios => Set<Horario>();
    public DbSet<Viatura> Viaturas => Set<Viatura>();
    public DbSet<Guarda> Guardas => Set<Guarda>();
    public DbSet<Equipe> Equipes => Set<Equipe>();
    public DbSet<EquipeMembro> EquipeMembros => Set<EquipeMembro>();
    public DbSet<Ferias> Ferias => Set<Ferias>();
    public DbSet<Ausencia> Ausencias => Set<Ausencia>();
    public DbSet<Escala> Escalas => Set<Escala>();
    public DbSet<EscalaItem> EscalaItens => Set<EscalaItem>();
    public DbSet<EscalaAlocacao> EscalaAlocacoes => Set<EscalaAlocacao>();
    public DbSet<Evento> Eventos => Set<Evento>();
    public DbSet<Ret> Rets => Set<Ret>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);
    }

    public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        foreach (var entry in ChangeTracker.Entries<AuditableEntity>())
        {
            if (entry.State == EntityState.Added)
            {
                entry.Entity.CreatedAt = DateTime.UtcNow;
            }
            else if (entry.State == EntityState.Modified)
            {
                entry.Entity.UpdatedAt = DateTime.UtcNow;
            }
        }
        return base.SaveChangesAsync(cancellationToken);
    }
}
