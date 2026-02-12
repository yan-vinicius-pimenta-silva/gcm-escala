using System.ComponentModel.DataAnnotations;
using EscalaGcm.Domain.Enums;

namespace EscalaGcm.Application.DTOs.Escalas;

public record EscalaDto(int Id, int Ano, int Mes, int Quinzena, int SetorId, string SetorNome, StatusEscala Status, List<EscalaItemDto> Itens);
public record EscalaItemDto(int Id, int EscalaId, string Data, int TurnoId, string TurnoNome, int HorarioId, string HorarioDescricao, string? Observacao, List<EscalaAlocacaoDto> Alocacoes);
public record EscalaAlocacaoDto(int Id, int? GuardaId, string? GuardaNome, int? EquipeId, string? EquipeNome, FuncaoAlocacao Funcao, int? ViaturaId, string? ViaturaIdentificador);

public record CreateEscalaRequest(int Ano, int Mes, int Quinzena, int SetorId);

public record AddEscalaItemRequest(
    [Required, StringLength(10)] string Data,
    int TurnoId, int HorarioId,
    [StringLength(144)] string? Observacao,
    List<AlocacaoRequest> Alocacoes);
public record AlocacaoRequest(int? GuardaId, int? EquipeId, FuncaoAlocacao Funcao, int? ViaturaId);

public record UpdateEscalaItemRequest(
    [Required, StringLength(10)] string Data,
    int TurnoId, int HorarioId,
    [StringLength(144)] string? Observacao,
    List<AlocacaoRequest> Alocacoes);

public record ConflictError(string Tipo, string Mensagem);
