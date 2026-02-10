using System.ComponentModel.DataAnnotations;

namespace EscalaGcm.Application.DTOs.Viaturas;

public record ViaturaDto(int Id, string Identificador, bool Ativo);
public record CreateViaturaRequest(
    [property: Required, StringLength(100)] string Identificador,
    bool Ativo = true);
public record UpdateViaturaRequest(
    [property: Required, StringLength(100)] string Identificador,
    bool Ativo);
