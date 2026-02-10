import { Box, Autocomplete, TextField, Select, MenuItem, FormControl, InputLabel, RadioGroup, FormControlLabel, Radio, Button, Chip } from '@mui/material';
import { useSetores } from '../setores/useSetores';
import type { Setor, StatusEscala } from '../../types';

const MESES = [
  'Janeiro', 'Fevereiro', 'Marco', 'Abril', 'Maio', 'Junho',
  'Julho', 'Agosto', 'Setembro', 'Outubro', 'Novembro', 'Dezembro',
];

const statusColor: Record<string, 'default' | 'primary' | 'success'> = {
  Rascunho: 'default',
  Publicada: 'primary',
  Fechada: 'success',
};

interface Props {
  setorId: number | null;
  onSetorChange: (id: number | null) => void;
  mes: number;
  onMesChange: (m: number) => void;
  ano: number;
  onAnoChange: (a: number) => void;
  quinzena: number;
  onQuinzenaChange: (q: number) => void;
  onCarregar: () => void;
  onPublicar: () => void;
  onFechar: () => void;
  status: StatusEscala | null;
  isLoading: boolean;
}

export default function EscalaFilters({
  setorId, onSetorChange, mes, onMesChange, ano, onAnoChange,
  quinzena, onQuinzenaChange, onCarregar, onPublicar, onFechar,
  status, isLoading,
}: Props) {
  const { data: setores = [] } = useSetores();
  const activeSetores = setores.filter((s: Setor) => s.ativo);
  const selectedSetor = activeSetores.find((s: Setor) => s.id === setorId) || null;

  return (
    <Box display="flex" gap={2} flexWrap="wrap" alignItems="center" mb={3}>
      <Autocomplete
        sx={{ minWidth: 220 }}
        options={activeSetores}
        getOptionLabel={(o: Setor) => o.nome}
        value={selectedSetor}
        onChange={(_, v) => onSetorChange(v?.id ?? null)}
        renderInput={(params) => <TextField {...params} label="Setor" size="small" />}
      />
      <FormControl size="small" sx={{ minWidth: 140 }}>
        <InputLabel>Mes</InputLabel>
        <Select value={mes} label="Mes" onChange={(e) => onMesChange(Number(e.target.value))}>
          {MESES.map((m, i) => <MenuItem key={i} value={i + 1}>{m}</MenuItem>)}
        </Select>
      </FormControl>
      <TextField
        label="Ano" type="number" size="small" sx={{ width: 100 }}
        value={ano} onChange={(e) => onAnoChange(Number(e.target.value))}
      />
      <RadioGroup row value={quinzena} onChange={(e) => onQuinzenaChange(Number(e.target.value))}>
        <FormControlLabel value={1} control={<Radio size="small" />} label="1a Quinzena" />
        <FormControlLabel value={2} control={<Radio size="small" />} label="2a Quinzena" />
      </RadioGroup>
      <Button variant="contained" onClick={onCarregar} disabled={isLoading || !setorId}>
        Carregar
      </Button>
      {status === 'Rascunho' && (
        <Button variant="outlined" color="success" onClick={onPublicar}>Publicar</Button>
      )}
      {status === 'Publicada' && (
        <Button variant="outlined" color="warning" onClick={onFechar}>Fechar</Button>
      )}
      {status && <Chip label={status} color={statusColor[status] || 'default'} />}
    </Box>
  );
}
