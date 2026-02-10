using System.ComponentModel.DataAnnotations;

namespace EscalaGcm.Application.DTOs.Horarios;

public record HorarioDto(int Id, string Inicio, string Fim, string Descricao, bool Ativo);
public record CreateHorarioRequest(
    [property: Required, StringLength(5)] string Inicio,
    [property: Required, StringLength(5)] string Fim,
    [property: StringLength(100)] string? Descricao,
    bool Ativo = true);
public record UpdateHorarioRequest(
    [property: Required, StringLength(5)] string Inicio,
    [property: Required, StringLength(5)] string Fim,
    [property: StringLength(100)] string? Descricao,
    bool Ativo);
