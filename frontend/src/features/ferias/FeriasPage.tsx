import { useState } from 'react';
import { DataGrid, type GridColDef } from '@mui/x-data-grid';
import { Box, IconButton } from '@mui/material';
import EditIcon from '@mui/icons-material/Edit';
import DeleteIcon from '@mui/icons-material/Delete';
import { useSnackbar } from 'notistack';
import PageHeader from '../../components/ui/PageHeader';
import ConfirmDialog from '../../components/ui/ConfirmDialog';
import FeriasForm from './FeriasForm';
import { useFeriasList, useCreateFerias, useUpdateFerias, useDeleteFerias } from './useFerias';
import type { Ferias } from '../../types';
import type { FeriasFormData } from './feriasSchema';
import { formatDateToDisplay } from '../../utils/date';

export default function FeriasPage() {
  const { data: items = [], isLoading } = useFeriasList();
  const createMut = useCreateFerias();
  const updateMut = useUpdateFerias();
  const deleteMut = useDeleteFerias();
  const { enqueueSnackbar } = useSnackbar();
  const [formOpen, setFormOpen] = useState(false);
  const [editItem, setEditItem] = useState<Ferias | null>(null);
  const [deleteId, setDeleteId] = useState<number | null>(null);

  const handleSubmit = async (data: FeriasFormData) => {
    try {
      if (editItem) { await updateMut.mutateAsync({ id: editItem.id, data }); enqueueSnackbar('Férias atualizada', { variant: 'success' }); }
      else { await createMut.mutateAsync(data); enqueueSnackbar('Férias criada', { variant: 'success' }); }
      setFormOpen(false);
    } catch (err: any) { enqueueSnackbar(err.response?.data?.message || 'Erro ao salvar', { variant: 'error' }); }
  };

  const handleDelete = async () => {
    if (deleteId == null) return;
    try { await deleteMut.mutateAsync(deleteId); enqueueSnackbar('Férias excluída', { variant: 'success' }); }
    catch (err: any) { enqueueSnackbar(err.response?.data?.message || 'Erro ao excluir', { variant: 'error' }); }
    setDeleteId(null);
  };

  const columns: GridColDef[] = [
    { field: 'id', headerName: 'ID', width: 70 },
    { field: 'guardaNome', headerName: 'Guarda', flex: 1 },
    { field: 'dataInicio', headerName: 'Início', width: 130, valueFormatter: (value) => formatDateToDisplay(value) },
    { field: 'dataFim', headerName: 'Fim', width: 130, valueFormatter: (value) => formatDateToDisplay(value) },
    { field: 'observacao', headerName: 'Observação', flex: 1 },
    { field: 'actions', headerName: 'Ações', width: 120, sortable: false, renderCell: (p) => (<><IconButton size="small" onClick={() => { setEditItem(p.row); setFormOpen(true); }}><EditIcon /></IconButton><IconButton size="small" onClick={() => setDeleteId(p.row.id)}><DeleteIcon /></IconButton></>) },
  ];

  return (
    <>
      <PageHeader title="Férias" onAdd={() => { setEditItem(null); setFormOpen(true); }} />
      <Box sx={{ width: '100%', overflowX: 'auto' }}>
        <DataGrid rows={items} columns={columns} loading={isLoading} autoHeight pageSizeOptions={[10, 25, 50]} initialState={{ pagination: { paginationModel: { pageSize: 10 } } }} />
      </Box>
      <FeriasForm open={formOpen} onClose={() => setFormOpen(false)} onSubmit={handleSubmit} editData={editItem} />
      <ConfirmDialog open={deleteId !== null} title="Excluir Férias" message="Tem certeza que deseja excluir estas férias?" onConfirm={handleDelete} onCancel={() => setDeleteId(null)} />
    </>
  );
}
