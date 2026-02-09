namespace EscalaGcm.Application.DTOs.Posicoes;

public record PosicaoDto(int Id, string Nome, bool Ativo);
public record CreatePosicaoRequest(string Nome, bool Ativo = true);
public record UpdatePosicaoRequest(string Nome, bool Ativo);
