using EscalaGcm.Domain.Entities;
using EscalaGcm.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace EscalaGcm.Infrastructure.Data;

public static class SeedData
{
    public static async Task InitializeAsync(AppDbContext context)
    {
        if (await context.Usuarios.AnyAsync())
            return;

        // Seed admin user (password: admin123)
        context.Usuarios.Add(new Usuario
        {
            NomeUsuario = "admin",
            SenhaHash = BCrypt.Net.BCrypt.HashPassword("admin123"),
            NomeCompleto = "Administrador",
            Perfil = PerfilUsuario.Admin,
            Ativo = true
        });

        // Seed predefined sectors
        var setores = new List<Setor>
        {
            new() { Nome = "Setor 1", Tipo = TipoSetor.Padrao, Ativo = true },
            new() { Nome = "Setor 2", Tipo = TipoSetor.Padrao, Ativo = true },
            new() { Nome = "Setor 3", Tipo = TipoSetor.Padrao, Ativo = true },
            new() { Nome = "Setor 4", Tipo = TipoSetor.Padrao, Ativo = true },
            new() { Nome = "Setor 5", Tipo = TipoSetor.Padrao, Ativo = true },
            new() { Nome = "Setor 6", Tipo = TipoSetor.Padrao, Ativo = true },
            new() { Nome = "Setor 7", Tipo = TipoSetor.Padrao, Ativo = true },
            new() { Nome = "Central de Comunicações 1", Tipo = TipoSetor.CentralComunicacoes, Ativo = true },
            new() { Nome = "Central de Comunicações 2", Tipo = TipoSetor.CentralComunicacoes, Ativo = true },
            new() { Nome = "Rádio Patrulha 01", Tipo = TipoSetor.RadioPatrulha, Ativo = true },
            new() { Nome = "Rádio Patrulha 02", Tipo = TipoSetor.RadioPatrulha, Ativo = true },
            new() { Nome = "Rádio Patrulha 03", Tipo = TipoSetor.RadioPatrulha, Ativo = true },
            new() { Nome = "Rádio Patrulha 04", Tipo = TipoSetor.RadioPatrulha, Ativo = true },
            new() { Nome = "Rádio Patrulha 05", Tipo = TipoSetor.RadioPatrulha, Ativo = true },
            new() { Nome = "Rádio Patrulha 06", Tipo = TipoSetor.RadioPatrulha, Ativo = true },
            new() { Nome = "Rádio Patrulha 07", Tipo = TipoSetor.RadioPatrulha, Ativo = true },
            new() { Nome = "Rádio Patrulha 08", Tipo = TipoSetor.RadioPatrulha, Ativo = true },
            new() { Nome = "Rádio Patrulha 09", Tipo = TipoSetor.RadioPatrulha, Ativo = true },
            new() { Nome = "Divisão Rural", Tipo = TipoSetor.DivisaoRural, Ativo = true },
            new() { Nome = "ROMU 1", Tipo = TipoSetor.Romu, Ativo = true },
            new() { Nome = "ROMU 2", Tipo = TipoSetor.Romu, Ativo = true },
            new() { Nome = "Ronda Comércio", Tipo = TipoSetor.RondaComercio, Ativo = true },
        };

        context.Setores.AddRange(setores);
        await context.SaveChangesAsync();
    }
}
