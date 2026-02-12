using System.ComponentModel.DataAnnotations;

namespace EscalaGcm.Application.DTOs.Posicoes;

public record PosicaoDto(int Id, string Nome, bool Ativo);
public record CreatePosicaoRequest(
    [Required, StringLength(144)] string Nome,
    bool Ativo = true);
public record UpdatePosicaoRequest(
    [Required, StringLength(144)] string Nome,
    bool Ativo);
