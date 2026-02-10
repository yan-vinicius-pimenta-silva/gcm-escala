import { useState } from 'react';
import { DataGrid, type GridColDef } from '@mui/x-data-grid';
import { IconButton } from '@mui/material';
import EditIcon from '@mui/icons-material/Edit';
import DeleteIcon from '@mui/icons-material/Delete';
import { useSnackbar } from 'notistack';
import PageHeader from '../../components/ui/PageHeader';
import ConfirmDialog from '../../components/ui/ConfirmDialog';
import AusenciaForm from './AusenciaForm';
import { useAusencias, useCreateAusencia, useUpdateAusencia, useDeleteAusencia } from './useAusencias';
import type { Ausencia } from '../../types';
import type { AusenciaFormData } from './ausenciaSchema';
import { formatDateToDisplay } from '../../utils/date';

export default function AusenciasPage() {
  const { data: items = [], isLoading } = useAusencias();
  const createMut = useCreateAusencia();
  const updateMut = useUpdateAusencia();
  const deleteMut = useDeleteAusencia();
  const { enqueueSnackbar } = useSnackbar();
  const [formOpen, setFormOpen] = useState(false);
  const [editItem, setEditItem] = useState<Ausencia | null>(null);
  const [deleteId, setDeleteId] = useState<number | null>(null);

  const handleSubmit = async (data: AusenciaFormData) => {
    try {
      if (editItem) { await updateMut.mutateAsync({ id: editItem.id, data }); enqueueSnackbar('Ausência atualizada', { variant: 'success' }); }
      else { await createMut.mutateAsync(data); enqueueSnackbar('Ausência criada', { variant: 'success' }); }
      setFormOpen(false);
    } catch (err: any) { enqueueSnackbar(err.response?.data?.message || 'Erro ao salvar', { variant: 'error' }); }
  };

  const handleDelete = async () => {
    if (deleteId == null) return;
    try { await deleteMut.mutateAsync(deleteId); enqueueSnackbar('Ausência excluída', { variant: 'success' }); }
    catch (err: any) { enqueueSnackbar(err.response?.data?.message || 'Erro ao excluir', { variant: 'error' }); }
    setDeleteId(null);
  };

  const columns: GridColDef[] = [
    { field: 'id', headerName: 'ID', width: 70 },
    { field: 'guardaNome', headerName: 'Guarda', flex: 1 },
    { field: 'dataInicio', headerName: 'Início', width: 130, valueFormatter: (value) => formatDateToDisplay(value) },
    { field: 'dataFim', headerName: 'Fim', width: 130, valueFormatter: (value) => formatDateToDisplay(value) },
    { field: 'motivo', headerName: 'Motivo', width: 180 },
    { field: 'observacoes', headerName: 'Observações', flex: 1 },
    { field: 'actions', headerName: 'Ações', width: 120, sortable: false, renderCell: (p) => (<><IconButton size="small" onClick={() => { setEditItem(p.row); setFormOpen(true); }}><EditIcon /></IconButton><IconButton size="small" onClick={() => setDeleteId(p.row.id)}><DeleteIcon /></IconButton></>) },
  ];

  return (
    <>
      <PageHeader title="Ausências" onAdd={() => { setEditItem(null); setFormOpen(true); }} />
      <DataGrid rows={items} columns={columns} loading={isLoading} autoHeight pageSizeOptions={[10, 25, 50]} initialState={{ pagination: { paginationModel: { pageSize: 10 } } }} />
      <AusenciaForm open={formOpen} onClose={() => setFormOpen(false)} onSubmit={handleSubmit} editData={editItem} />
      <ConfirmDialog open={deleteId !== null} title="Excluir Ausência" message="Tem certeza que deseja excluir esta ausência?" onConfirm={handleDelete} onCancel={() => setDeleteId(null)} />
    </>
  );
}
