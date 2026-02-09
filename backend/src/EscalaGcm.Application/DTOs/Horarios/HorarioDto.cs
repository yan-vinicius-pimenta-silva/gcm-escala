namespace EscalaGcm.Application.DTOs.Horarios;

public record HorarioDto(int Id, string Inicio, string Fim, string Descricao, bool Ativo);
public record CreateHorarioRequest(string Inicio, string Fim, string? Descricao, bool Ativo = true);
public record UpdateHorarioRequest(string Inicio, string Fim, string? Descricao, bool Ativo);
