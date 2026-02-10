using System.ComponentModel.DataAnnotations;

namespace EscalaGcm.Application.DTOs.Guardas;

public record GuardaDto(int Id, string Nome, string? Telefone, int PosicaoId, string? PosicaoNome, bool Ativo);
public record CreateGuardaRequest(
    [property: Required, StringLength(144)] string Nome,
    [property: StringLength(20)] string? Telefone,
    int PosicaoId, bool Ativo = true);
public record UpdateGuardaRequest(
    [property: Required, StringLength(144)] string Nome,
    [property: StringLength(20)] string? Telefone,
    int PosicaoId, bool Ativo);
