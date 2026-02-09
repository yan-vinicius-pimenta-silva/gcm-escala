namespace EscalaGcm.Application.DTOs.Guardas;

public record GuardaDto(int Id, string Nome, string? Telefone, int PosicaoId, string? PosicaoNome, bool Ativo);
public record CreateGuardaRequest(string Nome, string? Telefone, int PosicaoId, bool Ativo = true);
public record UpdateGuardaRequest(string Nome, string? Telefone, int PosicaoId, bool Ativo);
