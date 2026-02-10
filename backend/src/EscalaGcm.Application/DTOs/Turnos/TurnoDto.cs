using System.ComponentModel.DataAnnotations;

namespace EscalaGcm.Application.DTOs.Turnos;

public record TurnoDto(int Id, string Nome, bool Ativo);
public record CreateTurnoRequest(
    [property: Required, StringLength(144)] string Nome,
    bool Ativo = true);
public record UpdateTurnoRequest(
    [property: Required, StringLength(144)] string Nome,
    bool Ativo);
