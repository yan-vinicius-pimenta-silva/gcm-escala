import { DataGrid, type GridColDef } from '@mui/x-data-grid';
import { IconButton } from '@mui/material';
import EditIcon from '@mui/icons-material/Edit';
import DeleteIcon from '@mui/icons-material/Delete';
import type { EscalaItem, EscalaAlocacao } from '../../types';
import { formatDateToDisplay } from '../../utils/date';

function formatAlocados(alocacoes?: EscalaAlocacao[]): string {
  if (!alocacoes?.length) return '';
  return alocacoes.map(a => {
    if (a.guardaNome) return `${a.guardaNome} (${a.funcao})`;
    if (a.equipeNome) return `${a.equipeNome}`;
    return '';
  }).filter(Boolean).join(', ');
}

function formatIntegrantesEquipe(alocacoes?: EscalaAlocacao[]): string {
  if (!alocacoes?.length) return "";

  return alocacoes
    .map(a => {
      if (!a.equipeNome || !a.equipeNome.includes("[")) return "";
      const [nomeEquipe, integrantes] = a.equipeNome.split("[");
      return `${nomeEquipe.trim()}: ${integrantes.replace("]", "").trim()}`;
    })
    .filter(Boolean)
    .join(" | " );
}

function formatViatura(alocacoes?: EscalaAlocacao[]): string {
  if (!alocacoes?.length) return '';
  const v = alocacoes.find(a => a.viaturaIdentificador);
  return v?.viaturaIdentificador || '';
}

interface Props {
  items: EscalaItem[];
  onEdit: (item: EscalaItem) => void;
  onDelete: (itemId: number) => void;
  isReadOnly: boolean;
}

export default function EscalaGrid({ items, onEdit, onDelete, isReadOnly }: Props) {
  const rows = [...items]
    .sort((a, b) => a.data.localeCompare(b.data) || (a.turnoNome || '').localeCompare(b.turnoNome || ''))
    .map(item => ({
      id: item.id,
      data: item.data,
      turnoNome: item.turnoNome || '',
      horarioDescricao: item.horarioDescricao || '',
      regime: item.regime === 'Doze36' ? '12x36' : '8h',
      alocados: formatAlocados(item.alocacoes),
      viatura: formatViatura(item.alocacoes),
      integrantesEquipe: formatIntegrantesEquipe(item.alocacoes),
      observacao: item.observacao || '',
      _raw: item,
    }));

  const columns: GridColDef[] = [
    { field: 'data', headerName: 'Data', width: 120, valueFormatter: (value) => formatDateToDisplay(value) },
    { field: 'turnoNome', headerName: 'Turno', width: 120 },
    { field: 'horarioDescricao', headerName: 'Horario', width: 150 },
    { field: 'regime', headerName: 'Regime', width: 90 },
    { field: 'alocados', headerName: 'Alocados', flex: 1, minWidth: 200 },
    { field: 'viatura', headerName: 'Viatura', width: 120 },
    { field: 'integrantesEquipe', headerName: 'Integrantes da Equipe', flex: 1, minWidth: 230 },
    { field: 'observacao', headerName: 'Obs.', width: 150 },
  ];

  if (!isReadOnly) {
    columns.push({
      field: 'actions', headerName: 'Acoes', width: 120, sortable: false,
      renderCell: (p) => (
        <>
          <IconButton size="small" onClick={() => onEdit(p.row._raw)}><EditIcon /></IconButton>
          <IconButton size="small" onClick={() => onDelete(p.row.id)}><DeleteIcon /></IconButton>
        </>
      ),
    });
  }

  return (
    <DataGrid
      rows={rows} columns={columns} autoHeight
      pageSizeOptions={[10, 25, 50]}
      initialState={{ pagination: { paginationModel: { pageSize: 25 } } }}
    />
  );
}
