using System.ComponentModel.DataAnnotations;

namespace EscalaGcm.Application.DTOs.Guardas;

public record GuardaDto(int Id, string Nome, string? Telefone, int PosicaoId, string? PosicaoNome, bool Ativo);
public record CreateGuardaRequest(
    [Required, StringLength(144)] string Nome,
    [StringLength(20)] string? Telefone,
    int PosicaoId, bool Ativo = true);
public record UpdateGuardaRequest(
    [Required, StringLength(144)] string Nome,
    [StringLength(20)] string? Telefone,
    int PosicaoId, bool Ativo);
