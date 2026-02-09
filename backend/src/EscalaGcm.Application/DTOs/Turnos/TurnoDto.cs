namespace EscalaGcm.Application.DTOs.Turnos;

public record TurnoDto(int Id, string Nome, bool Ativo);
public record CreateTurnoRequest(string Nome, bool Ativo = true);
public record UpdateTurnoRequest(string Nome, bool Ativo);
