using EscalaGcm.Domain.Enums;

namespace EscalaGcm.Application.DTOs.Setores;

public record SetorDto(int Id, string Nome, TipoSetor Tipo, bool Ativo);
public record CreateSetorRequest(string Nome, TipoSetor Tipo, bool Ativo = true);
public record UpdateSetorRequest(string Nome, TipoSetor Tipo, bool Ativo);
