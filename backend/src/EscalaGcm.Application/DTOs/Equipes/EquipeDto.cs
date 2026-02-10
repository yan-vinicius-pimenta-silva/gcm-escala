using System.ComponentModel.DataAnnotations;

namespace EscalaGcm.Application.DTOs.Equipes;

public record EquipeDto(int Id, string Nome, bool Ativo, List<EquipeMembroDto> Membros);
public record EquipeMembroDto(int Id, int GuardaId, string GuardaNome);
public record CreateEquipeRequest(
    [property: Required, StringLength(144)] string Nome,
    bool Ativo, List<int> GuardaIds);
public record UpdateEquipeRequest(
    [property: Required, StringLength(144)] string Nome,
    bool Ativo, List<int> GuardaIds);
