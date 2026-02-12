using System.ComponentModel.DataAnnotations;
using EscalaGcm.Domain.Enums;

namespace EscalaGcm.Application.DTOs.Setores;

public record SetorDto(int Id, string Nome, TipoSetor Tipo, bool Ativo);
public record CreateSetorRequest(
    [Required, StringLength(144)] string Nome,
    TipoSetor Tipo, bool Ativo = true);
public record UpdateSetorRequest(
    [Required, StringLength(144)] string Nome,
    TipoSetor Tipo, bool Ativo);
