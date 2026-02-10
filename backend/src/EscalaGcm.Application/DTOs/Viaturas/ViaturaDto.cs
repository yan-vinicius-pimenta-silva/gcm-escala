namespace EscalaGcm.Application.DTOs.Viaturas;

public record ViaturaDto(int Id, string Identificador, bool Ativo);
public record CreateViaturaRequest(string Identificador, bool Ativo = true);
public record UpdateViaturaRequest(string Identificador, bool Ativo);
