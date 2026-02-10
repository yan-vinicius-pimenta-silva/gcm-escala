import { Box, FormControl, InputLabel, Select, MenuItem, TextField, Autocomplete, Button } from '@mui/material';
import DownloadIcon from '@mui/icons-material/Download';
import { useSetores } from '../setores/useSetores';
import { useGuardas } from '../guardas/useGuardas';
import type { TipoRelatorio } from '../../api/relatorios';
import type { Setor, Guarda } from '../../types';

const REPORT_OPTIONS: { value: TipoRelatorio; label: string }[] = [
  { value: 'EscalaMensalPorSetor', label: 'Escala Mensal por Setor' },
  { value: 'GuardasEscalados', label: 'Guardas Escalados' },
  { value: 'GuardasNaoEscalados', label: 'Guardas Nao Escalados' },
  { value: 'Ferias', label: 'Ferias' },
  { value: 'Ausencias', label: 'Ausencias' },
  { value: 'IndividualGuarda', label: 'Individual do Guarda' },
];

const MESES = [
  'Janeiro', 'Fevereiro', 'Marco', 'Abril', 'Maio', 'Junho',
  'Julho', 'Agosto', 'Setembro', 'Outubro', 'Novembro', 'Dezembro',
];

const SETOR_REPORTS: TipoRelatorio[] = ['EscalaMensalPorSetor', 'GuardasEscalados', 'GuardasNaoEscalados'];

interface Props {
  tipo: TipoRelatorio;
  onTipoChange: (t: TipoRelatorio) => void;
  mes: number;
  onMesChange: (m: number) => void;
  ano: number;
  onAnoChange: (a: number) => void;
  setorId: number | null;
  onSetorIdChange: (id: number | null) => void;
  guardaId: number | null;
  onGuardaIdChange: (id: number | null) => void;
  onGerar: () => void;
  onExportar: () => void;
  isLoading: boolean;
}

export default function RelatorioFilters({
  tipo, onTipoChange, mes, onMesChange, ano, onAnoChange,
  setorId, onSetorIdChange, guardaId, onGuardaIdChange,
  onGerar, onExportar, isLoading,
}: Props) {
  const { data: setores = [] } = useSetores();
  const { data: guardas = [] } = useGuardas();
  const activeSetores = setores.filter((s: Setor) => s.ativo);
  const activeGuardas = guardas.filter((g: Guarda) => g.ativo);

  const showSetor = SETOR_REPORTS.includes(tipo);
  const showGuarda = tipo === 'IndividualGuarda';

  return (
    <Box display="flex" gap={2} flexWrap="wrap" alignItems="center" mb={3}>
      <FormControl size="small" sx={{ minWidth: 220 }}>
        <InputLabel>Tipo de Relatorio</InputLabel>
        <Select value={tipo} label="Tipo de Relatorio" onChange={(e) => onTipoChange(e.target.value as TipoRelatorio)}>
          {REPORT_OPTIONS.map(o => <MenuItem key={o.value} value={o.value}>{o.label}</MenuItem>)}
        </Select>
      </FormControl>
      <FormControl size="small" sx={{ minWidth: 140 }}>
        <InputLabel>Mes</InputLabel>
        <Select value={mes} label="Mes" onChange={(e) => onMesChange(Number(e.target.value))}>
          {MESES.map((m, i) => <MenuItem key={i} value={i + 1}>{m}</MenuItem>)}
        </Select>
      </FormControl>
      <TextField label="Ano" type="number" size="small" sx={{ width: 100 }} value={ano} onChange={(e) => onAnoChange(Number(e.target.value))} />

      {showSetor && (
        <Autocomplete
          sx={{ minWidth: 200 }}
          options={activeSetores}
          getOptionLabel={(o: Setor) => o.nome}
          value={activeSetores.find(s => s.id === setorId) || null}
          onChange={(_, v) => onSetorIdChange(v?.id ?? null)}
          renderInput={(params) => <TextField {...params} label="Setor (opcional)" size="small" />}
        />
      )}

      {showGuarda && (
        <Autocomplete
          sx={{ minWidth: 200 }}
          options={activeGuardas}
          getOptionLabel={(o: Guarda) => o.nome}
          value={activeGuardas.find(g => g.id === guardaId) || null}
          onChange={(_, v) => onGuardaIdChange(v?.id ?? null)}
          renderInput={(params) => <TextField {...params} label="Guarda" size="small" />}
        />
      )}

      <Button variant="contained" onClick={onGerar} disabled={isLoading || (showGuarda && !guardaId)}>
        Gerar
      </Button>
      <Button variant="outlined" startIcon={<DownloadIcon />} onClick={onExportar} disabled={isLoading}>
        Exportar Excel
      </Button>
    </Box>
  );
}
