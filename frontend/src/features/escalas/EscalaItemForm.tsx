import { useState, useEffect, useMemo } from 'react';
import {
  Box, Paper, Typography, FormControl, InputLabel, Select, MenuItem,
  TextField, Button, Chip, Stack, RadioGroup, FormControlLabel, Radio,
  FormLabel, Tooltip,
} from '@mui/material';
import { useQuery } from '@tanstack/react-query';
import { useTurnos } from '../turnos/useTurnos';
import { useHorarios } from '../horarios/useHorarios';
import AlocacaoFields, { emptyAlocacaoState, buildAlocacoes } from './AlocacaoFields';
import type { AlocacaoState } from './AlocacaoFields';
import { RegimeTrabalho, StatusDisponibilidade } from '../../types';
import type { TipoSetor, EscalaItem, Turno, Horario, DayAvailabilityDto } from '../../types';
import type { AddEscalaItemRequest } from '../../api/escalas';
import { getGuardaDisponibilidade } from '../../api/escalas';

interface Props {
  tipoSetor: TipoSetor;
  ano: number;
  mes: number;
  quinzena: number;
  editingItem: EscalaItem | null;
  onSubmit: (items: AddEscalaItemRequest[]) => void;
  onCancelEdit: () => void;
  disabled: boolean;
}

function getDaysInQuinzena(ano: number, mes: number, quinzena: number): number[] {
  if (quinzena === 1) return Array.from({ length: 15 }, (_, i) => i + 1);
  const lastDay = new Date(ano, mes, 0).getDate();
  return Array.from({ length: lastDay - 15 }, (_, i) => i + 16);
}

function formatDate(ano: number, mes: number, day: number): string {
  return `${ano}-${String(mes).padStart(2, '0')}-${String(day).padStart(2, '0')}`;
}

function horarioDuracaoMinutos(horario: Horario): number {
  const [startH, startM] = horario.inicio.split(':').map(Number);
  const [endH, endM] = horario.fim.split(':').map(Number);
  const startMin = startH * 60 + startM;
  const endMin = endH * 60 + endM;
  return endMin > startMin ? endMin - startMin : 24 * 60 - startMin + endMin;
}

function isHorarioCompativelComRegime(horario: Horario, regime: RegimeTrabalho): boolean {
  const duracao = horarioDuracaoMinutos(horario);
  return regime === RegimeTrabalho.Doze36 ? duracao >= 600 : duracao < 600;
}

function getGuardaIdFromAlocState(alocState: AlocacaoState): number | null {
  if (alocState.guardaIds.length > 0) return alocState.guardaIds[0];
  if (alocState.motoristaId) return alocState.motoristaId;
  if (alocState.encarregadoId) return alocState.encarregadoId;
  if (alocState.apoioGuardaIds.length > 0) return alocState.apoioGuardaIds[0];
  return null;
}

interface LocalBlockInfo {
  reason: string;
  availableFrom?: string; // "HH:mm" — dia parcialmente bloqueado, guarda livre a partir deste horário
}

function chipColor(status: StatusDisponibilidade, selected: boolean, localBlock?: LocalBlockInfo) {
  if (selected) return 'primary';
  if (localBlock) return localBlock.availableFrom ? 'warning' : 'error';
  if (status === StatusDisponibilidade.Bloqueado) return 'error';
  if (status === StatusDisponibilidade.Parcial) return 'warning';
  return 'default';
}

function chipTooltip(info: DayAvailabilityDto | undefined, selected: boolean, localBlock?: LocalBlockInfo): string {
  if (selected) return 'Dia já adicionado — clique para remover';
  if (localBlock) {
    return localBlock.availableFrom
      ? `⚠️ ${localBlock.reason}`
      : `Indisponível — ${localBlock.reason}`;
  }
  if (!info) return 'Disponível';
  if (info.status === StatusDisponibilidade.Bloqueado) return `Indisponível — ${info.motivo ?? 'guarda ocupado o dia todo'}`;
  if (info.status === StatusDisponibilidade.Parcial) return `⚠️ ${info.motivo ?? `Disponível a partir das ${info.disponibilidadeAPartirDe}`}`;
  return 'Disponível';
}

export default function EscalaItemForm({ tipoSetor, ano, mes, quinzena, editingItem, onSubmit, onCancelEdit, disabled }: Props) {
  const { data: turnos = [] } = useTurnos();
  const { data: horarios = [] } = useHorarios();
  const activeTurnos = turnos.filter((t: Turno) => t.ativo);
  const activeHorarios = horarios.filter((h: Horario) => h.ativo);

  const [regime, setRegime] = useState<RegimeTrabalho>(RegimeTrabalho.Doze36);
  const [turnoId, setTurnoId] = useState<number | ''>('');
  const [horarioId, setHorarioId] = useState<number | ''>('');

  // Horários compatíveis com o regime selecionado (12x36 ↔ ≥10h; 8h ↔ <10h)
  const compatibleHorarios = activeHorarios.filter((h: Horario) => isHorarioCompativelComRegime(h, regime));
  const [selectedDays, setSelectedDays] = useState<number[]>([]);
  const [observacao, setObservacao] = useState('');
  const [alocState, setAlocState] = useState<AlocacaoState>(emptyAlocacaoState);

  const days = getDaysInQuinzena(ano, mes, quinzena);

  // Determine a guard to check availability for
  const primaryGuardaId = getGuardaIdFromAlocState(alocState);

  // Fetch availability when a guard is selected
  const { data: availability = [] } = useQuery<DayAvailabilityDto[]>({
    queryKey: ['guarda-disponibilidade', primaryGuardaId, ano, mes],
    queryFn: () => getGuardaDisponibilidade(primaryGuardaId!, ano, mes),
    enabled: primaryGuardaId != null && !disabled,
    staleTime: 30_000,
  });

  // Map day → availability info
  const availabilityMap = useMemo(() => {
    const map = new Map<number, DayAvailabilityDto>();
    for (const info of availability) map.set(info.dia, info);
    return map;
  }, [availability]);

  // Dias localmente bloqueados/parciais pela folga obrigatória 12x36 dos dias já selecionados
  const locallyBlockedDays = useMemo(() => {
    if (regime !== RegimeTrabalho.Doze36 || !horarioId || selectedDays.length === 0) {
      return new Map<number, LocalBlockInfo>();
    }
    const horario = activeHorarios.find((h: Horario) => h.id === horarioId);
    if (!horario) return new Map<number, LocalBlockInfo>();

    const blocked = new Map<number, LocalBlockInfo>();
    const [startH, startM] = horario.inicio.split(':').map(Number);
    const [endH, endM] = horario.fim.split(':').map(Number);

    function occ(day: number) {
      const shiftStart = new Date(ano, mes - 1, day, startH, startM);
      const shiftEnd = (endH > startH || (endH === startH && endM >= startM))
        ? new Date(ano, mes - 1, day, endH, endM)
        : new Date(ano, mes - 1, day + 1, endH, endM);
      return { shiftStart, occupiedTo: new Date(shiftEnd.getTime() + 36 * 3_600_000) };
    }

    const selOccs = selectedDays.map(d => ({ day: d, ...occ(d) }));

    for (const checkDay of days) {
      if (selectedDays.includes(checkDay)) continue;
      const checkDayStart = new Date(ano, mes - 1, checkDay);
      const checkDayEnd = new Date(ano, mes - 1, checkDay + 1);

      // Verificação 1 (para frente): o período de folga de um dia selecionado cobre checkDay?
      for (const { day: selDay, shiftStart, occupiedTo } of selOccs) {
        if (shiftStart >= checkDayEnd || occupiedTo <= checkDayStart) continue;
        const existing = blocked.get(checkDay);
        if (occupiedTo >= checkDayEnd) {
          if (!existing || existing.availableFrom !== undefined) {
            blocked.set(checkDay, { reason: `Folga obrigatória 12x36 após o turno do dia ${selDay}` });
          }
        } else if (!existing) {
          const hh = String(occupiedTo.getHours()).padStart(2, '0');
          const mm = String(occupiedTo.getMinutes()).padStart(2, '0');
          blocked.set(checkDay, {
            reason: `Folga 12x36 — disponível a partir das ${hh}:${mm}`,
            availableFrom: `${hh}:${mm}`,
          });
        }
      }

      // Se já está totalmente bloqueado, não precisa da verificação 2
      if (blocked.has(checkDay) && !blocked.get(checkDay)!.availableFrom) continue;

      // Verificação 2 (para trás): o período de folga de checkDay cobriria algum dia já selecionado?
      // Espelha a Verificação 1: bloqueio total se ultrapassa selShiftStart,
      // bloqueio parcial se termina dentro do selDay mas antes do turno.
      const { shiftStart: cStart, occupiedTo: cOccTo } = occ(checkDay);
      for (const { day: selDay, shiftStart: selShiftStart } of selOccs) {
        if (cStart >= selShiftStart) continue; // checkDay deve ser anterior a selDay
        const selDayStart = new Date(ano, mes - 1, selDay);

        if (cOccTo > selShiftStart) {
          // Bloqueio total: folga ultrapassa o turno do dia selecionado
          blocked.set(checkDay, { reason: `Conflito com o dia ${selDay} já escalado` });
          break;
        } else if (cOccTo > selDayStart) {
          // Bloqueio parcial: folga termina dentro do selDay, antes do turno
          if (!blocked.has(checkDay)) {
            const hh = String(cOccTo.getHours()).padStart(2, '0');
            const mm = String(cOccTo.getMinutes()).padStart(2, '0');
            blocked.set(checkDay, {
              reason: `Folga 12x36 — disponível a partir das ${hh}:${mm}`,
              availableFrom: `${hh}:${mm}`,
            });
          }
        }
      }
    }
    return blocked;
  }, [selectedDays, regime, horarioId, activeHorarios, days, ano, mes]);

  // Limpa horário selecionado se ele for incompatível com o novo regime
  useEffect(() => {
    if (!horarioId) return;
    const horario = activeHorarios.find((h: Horario) => h.id === horarioId);
    if (horario && !isHorarioCompativelComRegime(horario, regime)) {
      setHorarioId('');
    }
  }, [regime]);

  useEffect(() => {
    if (editingItem) {
      setRegime(editingItem.regime ?? RegimeTrabalho.Doze36);
      setTurnoId(editingItem.turnoId);
      setHorarioId(editingItem.horarioId);
      const itemDay = new Date(editingItem.data + 'T00:00:00').getDate();
      setSelectedDays([itemDay]);
      setObservacao(editingItem.observacao || '');
      const state: AlocacaoState = { ...emptyAlocacaoState };
      if (editingItem.alocacoes) {
        for (const a of editingItem.alocacoes) {
          if (a.funcao === 'Integrante' && a.guardaId) state.guardaIds.push(a.guardaId);
          else if (a.funcao === 'Motorista' && a.guardaId) state.motoristaId = a.guardaId;
          else if (a.funcao === 'Encarregado' && a.guardaId) state.encarregadoId = a.guardaId;
          else if (a.funcao === 'Apoio' && a.guardaId) state.apoioGuardaIds.push(a.guardaId);
          if (a.equipeId) state.equipeId = a.equipeId;
          if (a.viaturaId) state.viaturaId = a.viaturaId;
        }
      }
      setAlocState({ ...state });
    } else {
      resetForm();
    }
  }, [editingItem]);

  function resetForm() {
    setRegime(RegimeTrabalho.Doze36);
    setTurnoId('');
    setHorarioId('');
    setSelectedDays([]);
    setObservacao('');
    setAlocState(emptyAlocacaoState);
  }

  function toggleDay(day: number) {
    if (editingItem) return;
    if (daysLocked) return;
    const info = availabilityMap.get(day);
    if (info?.status === StatusDisponibilidade.Bloqueado) return;
    const localBlock = locallyBlockedDays.get(day);
    if (localBlock && !localBlock.availableFrom) return; // bloqueio total: não clicável
    setSelectedDays(prev => prev.includes(day) ? prev.filter(d => d !== day) : [...prev, day]);
  }

  function handleSubmit() {
    if (!turnoId || !horarioId || selectedDays.length === 0) return;
    const alocacoes = buildAlocacoes(tipoSetor, alocState);
    const items: AddEscalaItemRequest[] = selectedDays.map(day => ({
      data: formatDate(ano, mes, day),
      turnoId: turnoId as number,
      horarioId: horarioId as number,
      regime,
      observacao: observacao || undefined,
      alocacoes,
    }));
    onSubmit(items);
  }

  const isEditing = editingItem !== null;
  // Dias só ficam disponíveis após regime, turno e horário estarem selecionados
  const daysLocked = !turnoId || !horarioId;

  // "Todos": para 12x36 usa algoritmo guloso (ordem crescente, respeitando folgas)
  function selectAllAvailable() {
    const serverBlocked = (d: number) => availabilityMap.get(d)?.status === StatusDisponibilidade.Bloqueado;

    if (regime !== RegimeTrabalho.Doze36 || !horarioId) {
      setSelectedDays(days.filter(d => !serverBlocked(d)));
      return;
    }

    const horario = activeHorarios.find((h: Horario) => h.id === horarioId);
    if (!horario) return;

    const [startH, startM] = horario.inicio.split(':').map(Number);
    const [endH, endM] = horario.fim.split(':').map(Number);

    function occ(day: number) {
      const shiftStart = new Date(ano, mes - 1, day, startH, startM);
      const shiftEnd = (endH > startH || (endH === startH && endM >= startM))
        ? new Date(ano, mes - 1, day, endH, endM)
        : new Date(ano, mes - 1, day + 1, endH, endM);
      return { shiftStart, occupiedTo: new Date(shiftEnd.getTime() + 36 * 3_600_000) };
    }

    const selected: number[] = [];
    for (const day of days) {
      if (serverBlocked(day)) continue;
      const { shiftStart: dShiftStart } = occ(day);
      // Conflito: algum dia já selecionado ainda está em folga quando este turno começa
      const conflicts = selected.some(selDay => occ(selDay).occupiedTo > dShiftStart);
      if (!conflicts) selected.push(day);
    }
    setSelectedDays(selected);
  }

  return (
    <Paper sx={{ p: 2 }}>
      <Typography variant="h6" mb={2}>{isEditing ? 'Editar Lancamento' : 'Adicionar Lancamento'}</Typography>

      <Box display="flex" flexDirection="column" gap={2}>
        <FormControl component="fieldset" disabled={disabled || isEditing}>
          <FormLabel component="legend" sx={{ fontSize: '0.75rem' }}>Regime de Trabalho</FormLabel>
          <RadioGroup
            row
            value={regime}
            onChange={(e) => setRegime(e.target.value as RegimeTrabalho)}
          >
            <FormControlLabel value={RegimeTrabalho.Doze36} control={<Radio size="small" />} label="12x36" />
            <FormControlLabel value={RegimeTrabalho.OitoHoras} control={<Radio size="small" />} label="8h Diárias" />
          </RadioGroup>
        </FormControl>

        <FormControl size="small" fullWidth disabled={disabled}>
          <InputLabel>Turno</InputLabel>
          <Select value={turnoId} label="Turno" onChange={(e) => setTurnoId(Number(e.target.value))}>
            {activeTurnos.map((t: Turno) => <MenuItem key={t.id} value={t.id}>{t.nome}</MenuItem>)}
          </Select>
        </FormControl>

        <FormControl size="small" fullWidth disabled={disabled}>
          <InputLabel>Horario</InputLabel>
          <Select value={horarioId} label="Horario" onChange={(e) => setHorarioId(Number(e.target.value))}>
            {compatibleHorarios.map((h: Horario) => <MenuItem key={h.id} value={h.id}>{h.descricao || `${h.inicio} - ${h.fim}`}</MenuItem>)}
          </Select>
        </FormControl>

        <AlocacaoFields tipoSetor={tipoSetor} alocacoes={alocState} onChange={setAlocState} />

        <Box>
          <Box display="flex" justifyContent="space-between" alignItems="center" mb={0.5}>
            <Typography variant="subtitle2">
              Dias
              {primaryGuardaId && availability.length > 0 && !daysLocked && (
                <Typography component="span" variant="caption" ml={1} color="text.secondary">
                  (disponibilidade calculada)
                </Typography>
              )}
            </Typography>
            {!isEditing && !disabled && !daysLocked && (
              <Box>
                <Button size="small" onClick={selectAllAvailable}>Todos</Button>
                <Button size="small" onClick={() => setSelectedDays([])}>Limpar</Button>
              </Box>
            )}
          </Box>
          {daysLocked && !isEditing && !disabled ? (
            <Typography variant="caption" color="text.disabled">
              Selecione o turno e o horário para habilitar a escolha de dias.
            </Typography>
          ) : (
            <>
              {primaryGuardaId && availability.length > 0 && (
                <Box display="flex" gap={1} mb={0.5} flexWrap="wrap">
                  <Typography variant="caption" sx={{ display: 'flex', alignItems: 'center', gap: 0.5 }}>
                    <Box component="span" sx={{ width: 10, height: 10, borderRadius: '50%', bgcolor: 'success.main', display: 'inline-block' }} /> Disponível
                  </Typography>
                  <Typography variant="caption" sx={{ display: 'flex', alignItems: 'center', gap: 0.5 }}>
                    <Box component="span" sx={{ width: 10, height: 10, borderRadius: '50%', bgcolor: 'warning.main', display: 'inline-block' }} /> Parcial
                  </Typography>
                  <Typography variant="caption" sx={{ display: 'flex', alignItems: 'center', gap: 0.5 }}>
                    <Box component="span" sx={{ width: 10, height: 10, borderRadius: '50%', bgcolor: 'error.main', display: 'inline-block' }} /> Bloqueado
                  </Typography>
                </Box>
              )}
              <Stack direction="row" flexWrap="wrap" gap={0.5}>
                {days.map(day => {
                  const info = availabilityMap.get(day);
                  const selected = selectedDays.includes(day);
                  const localBlock = locallyBlockedDays.get(day);
                  const fullyBlocked = info?.status === StatusDisponibilidade.Bloqueado || (!!localBlock && !localBlock.availableFrom);
                  const color = chipColor(info?.status ?? StatusDisponibilidade.Disponivel, selected, localBlock);
                  const tooltip = chipTooltip(info, selected, localBlock);
                  return (
                    <Tooltip key={day} title={tooltip} arrow placement="top">
                      <span>
                        <Chip
                          label={day}
                          size="small"
                          color={color}
                          onClick={() => !disabled && toggleDay(day)}
                          variant={selected ? 'filled' : 'outlined'}
                          disabled={disabled || fullyBlocked || (isEditing && !selected)}
                          sx={fullyBlocked ? { cursor: 'not-allowed', opacity: 0.6 } : undefined}
                        />
                      </span>
                    </Tooltip>
                  );
                })}
              </Stack>
            </>
          )}
        </Box>

        <TextField
          label="Observacao" size="small" fullWidth multiline rows={2}
          value={observacao} onChange={(e) => setObservacao(e.target.value)} disabled={disabled}
          inputProps={{ maxLength: 144 }}
        />

        <Box display="flex" gap={1}>
          <Button
            variant="contained" onClick={handleSubmit} disabled={disabled || !turnoId || !horarioId || selectedDays.length === 0}
          >
            {isEditing ? 'Salvar Alteracoes' : 'Adicionar a Escala'}
          </Button>
          {isEditing && (
            <Button variant="outlined" onClick={() => { resetForm(); onCancelEdit(); }}>
              Cancelar
            </Button>
          )}
        </Box>
      </Box>
    </Paper>
  );
}
