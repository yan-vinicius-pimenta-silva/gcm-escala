using System.ComponentModel.DataAnnotations;

namespace EscalaGcm.Application.DTOs.Horarios;

public record HorarioDto(int Id, string Inicio, string Fim, string Descricao, bool Ativo);
public record CreateHorarioRequest(
    [Required, StringLength(5)] string Inicio,
    [Required, StringLength(5)] string Fim,
    [StringLength(100)] string? Descricao,
    bool Ativo = true);
public record UpdateHorarioRequest(
    [Required, StringLength(5)] string Inicio,
    [Required, StringLength(5)] string Fim,
    [StringLength(100)] string? Descricao,
    bool Ativo);
