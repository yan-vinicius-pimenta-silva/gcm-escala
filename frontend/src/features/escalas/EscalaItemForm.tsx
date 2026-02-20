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

function getGuardaIdFromAlocState(alocState: AlocacaoState): number | null {
  if (alocState.guardaIds.length > 0) return alocState.guardaIds[0];
  if (alocState.motoristaId) return alocState.motoristaId;
  if (alocState.encarregadoId) return alocState.encarregadoId;
  if (alocState.apoioGuardaIds.length > 0) return alocState.apoioGuardaIds[0];
  return null;
}

function chipColor(status: StatusDisponibilidade, selected: boolean) {
  if (selected) return 'primary';
  if (status === StatusDisponibilidade.Bloqueado) return 'error';
  if (status === StatusDisponibilidade.Parcial) return 'warning';
  return 'default';
}

function chipTooltip(info: DayAvailabilityDto | undefined, selected: boolean): string {
  if (selected) return 'Dia já adicionado — clique para remover';
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
    const info = availabilityMap.get(day);
    if (info?.status === StatusDisponibilidade.Bloqueado) return;
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

  // "Todos" selects only available/parcial days
  function selectAllAvailable() {
    setSelectedDays(days.filter(d => {
      const info = availabilityMap.get(d);
      return !info || info.status !== StatusDisponibilidade.Bloqueado;
    }));
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
            {activeHorarios.map((h: Horario) => <MenuItem key={h.id} value={h.id}>{h.descricao || `${h.inicio} - ${h.fim}`}</MenuItem>)}
          </Select>
        </FormControl>

        <AlocacaoFields tipoSetor={tipoSetor} alocacoes={alocState} onChange={setAlocState} />

        <Box>
          <Box display="flex" justifyContent="space-between" alignItems="center" mb={0.5}>
            <Typography variant="subtitle2">
              Dias
              {primaryGuardaId && availability.length > 0 && (
                <Typography component="span" variant="caption" ml={1} color="text.secondary">
                  (disponibilidade calculada)
                </Typography>
              )}
            </Typography>
            {!isEditing && !disabled && (
              <Box>
                <Button size="small" onClick={selectAllAvailable}>Todos</Button>
                <Button size="small" onClick={() => setSelectedDays([])}>Limpar</Button>
              </Box>
            )}
          </Box>
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
              const blocked = info?.status === StatusDisponibilidade.Bloqueado;
              const color = chipColor(info?.status ?? StatusDisponibilidade.Disponivel, selected);
              const tooltip = chipTooltip(info, selected);
              return (
                <Tooltip key={day} title={tooltip} arrow placement="top">
                  <span>
                    <Chip
                      label={day}
                      size="small"
                      color={color}
                      onClick={() => !disabled && toggleDay(day)}
                      variant={selected ? 'filled' : 'outlined'}
                      disabled={disabled || blocked || (isEditing && !selected)}
                      sx={blocked ? { cursor: 'not-allowed', opacity: 0.6 } : undefined}
                    />
                  </span>
                </Tooltip>
              );
            })}
          </Stack>
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
