import { useState, useEffect } from 'react';
import { Box, Paper, Typography, FormControl, InputLabel, Select, MenuItem, TextField, Button, Chip, Stack } from '@mui/material';
import { useTurnos } from '../turnos/useTurnos';
import { useHorarios } from '../horarios/useHorarios';
import AlocacaoFields, { emptyAlocacaoState, buildAlocacoes } from './AlocacaoFields';
import type { AlocacaoState } from './AlocacaoFields';
import type { TipoSetor, EscalaItem, Turno, Horario } from '../../types';
import type { AddEscalaItemRequest } from '../../api/escalas';

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

export default function EscalaItemForm({ tipoSetor, ano, mes, quinzena, editingItem, onSubmit, onCancelEdit, disabled }: Props) {
  const { data: turnos = [] } = useTurnos();
  const { data: horarios = [] } = useHorarios();
  const activeTurnos = turnos.filter((t: Turno) => t.ativo);
  const activeHorarios = horarios.filter((h: Horario) => h.ativo);

  const [turnoId, setTurnoId] = useState<number | ''>('');
  const [horarioId, setHorarioId] = useState<number | ''>('');
  const [selectedDays, setSelectedDays] = useState<number[]>([]);
  const [observacao, setObservacao] = useState('');
  const [alocState, setAlocState] = useState<AlocacaoState>(emptyAlocacaoState);

  const days = getDaysInQuinzena(ano, mes, quinzena);

  useEffect(() => {
    if (editingItem) {
      setTurnoId(editingItem.turnoId);
      setHorarioId(editingItem.horarioId);
      const itemDay = new Date(editingItem.data + 'T00:00:00').getDate();
      setSelectedDays([itemDay]);
      setObservacao(editingItem.observacao || '');
      // Pre-fill allocation from existing item
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
    setTurnoId('');
    setHorarioId('');
    setSelectedDays([]);
    setObservacao('');
    setAlocState(emptyAlocacaoState);
  }

  function toggleDay(day: number) {
    if (editingItem) return;
    setSelectedDays(prev => prev.includes(day) ? prev.filter(d => d !== day) : [...prev, day]);
  }

  function handleSubmit() {
    if (!turnoId || !horarioId || selectedDays.length === 0) return;
    const alocacoes = buildAlocacoes(tipoSetor, alocState);
    const items: AddEscalaItemRequest[] = selectedDays.map(day => ({
      data: formatDate(ano, mes, day),
      turnoId: turnoId as number,
      horarioId: horarioId as number,
      observacao: observacao || undefined,
      alocacoes,
    }));
    onSubmit(items);
  }

  const isEditing = editingItem !== null;

  return (
    <Paper sx={{ p: 2 }}>
      <Typography variant="h6" mb={2}>{isEditing ? 'Editar Lancamento' : 'Adicionar Lancamento'}</Typography>

      <Box display="flex" flexDirection="column" gap={2}>
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

        <Box>
          <Box display="flex" justifyContent="space-between" alignItems="center" mb={1}>
            <Typography variant="subtitle2">Dias</Typography>
            {!isEditing && !disabled && (
              <Box>
                <Button size="small" onClick={() => setSelectedDays(days)}>Todos</Button>
                <Button size="small" onClick={() => setSelectedDays([])}>Limpar</Button>
              </Box>
            )}
          </Box>
          <Stack direction="row" flexWrap="wrap" gap={0.5}>
            {days.map(day => (
              <Chip
                key={day}
                label={day}
                size="small"
                color={selectedDays.includes(day) ? 'primary' : 'default'}
                onClick={() => !disabled && toggleDay(day)}
                variant={selectedDays.includes(day) ? 'filled' : 'outlined'}
                disabled={disabled || (isEditing && !selectedDays.includes(day))}
              />
            ))}
          </Stack>
        </Box>

        <AlocacaoFields tipoSetor={tipoSetor} alocacoes={alocState} onChange={setAlocState} />

        <TextField
          label="Observacao" size="small" fullWidth multiline rows={2}
          value={observacao} onChange={(e) => setObservacao(e.target.value)} disabled={disabled}
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
