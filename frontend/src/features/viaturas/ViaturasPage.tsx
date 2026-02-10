import { useState } from 'react';
import { DataGrid, type GridColDef } from '@mui/x-data-grid';
import { Chip, IconButton } from '@mui/material';
import EditIcon from '@mui/icons-material/Edit';
import DeleteIcon from '@mui/icons-material/Delete';
import { useSnackbar } from 'notistack';
import PageHeader from '../../components/ui/PageHeader';
import ConfirmDialog from '../../components/ui/ConfirmDialog';
import ViaturaForm from './ViaturaForm';
import { useViaturas, useCreateViatura, useUpdateViatura, useDeleteViatura } from './useViaturas';
import type { Viatura } from '../../types';
import type { ViaturaFormData } from './viaturaSchema';

export default function ViaturasPage() {
  const { data: items = [], isLoading } = useViaturas();
  const createMut = useCreateViatura();
  const updateMut = useUpdateViatura();
  const deleteMut = useDeleteViatura();
  const { enqueueSnackbar } = useSnackbar();
  const [formOpen, setFormOpen] = useState(false);
  const [editItem, setEditItem] = useState<Viatura | null>(null);
  const [deleteId, setDeleteId] = useState<number | null>(null);
  const handleSubmit = async (data: ViaturaFormData) => {
    try {
      if (editItem) { await updateMut.mutateAsync({ id: editItem.id, data }); enqueueSnackbar('Viatura atualizada', { variant: 'success' }); }
      else { await createMut.mutateAsync(data); enqueueSnackbar('Viatura criada', { variant: 'success' }); }
      setFormOpen(false);
    } catch (err: any) { enqueueSnackbar(err.response?.data?.message || 'Erro ao salvar', { variant: 'error' }); }
  };
  const handleDelete = async () => {
    if (deleteId == null) return;
    try { await deleteMut.mutateAsync(deleteId); enqueueSnackbar('Viatura excluída', { variant: 'success' }); }
    catch (err: any) { enqueueSnackbar(err.response?.data?.message || 'Erro ao excluir', { variant: 'error' }); }
    setDeleteId(null);
  };
  const columns: GridColDef[] = [
    { field: 'id', headerName: 'ID', width: 70 },
    { field: 'identificador', headerName: 'Identificador', flex: 1 },
    { field: 'ativo', headerName: 'Status', width: 100, renderCell: (p) => <Chip label={p.value ? 'Ativo' : 'Inativo'} color={p.value ? 'success' : 'default'} size="small" /> },
    { field: 'actions', headerName: 'Ações', width: 120, sortable: false, renderCell: (p) => (<><IconButton size="small" onClick={() => { setEditItem(p.row); setFormOpen(true); }}><EditIcon /></IconButton><IconButton size="small" onClick={() => setDeleteId(p.row.id)}><DeleteIcon /></IconButton></>) },
  ];
  return (
    <>
      <PageHeader title="Viaturas" onAdd={() => { setEditItem(null); setFormOpen(true); }} />
      <DataGrid rows={items} columns={columns} loading={isLoading} autoHeight pageSizeOptions={[10, 25, 50]} initialState={{ pagination: { paginationModel: { pageSize: 10 } } }} />
      <ViaturaForm open={formOpen} onClose={() => setFormOpen(false)} onSubmit={handleSubmit} editData={editItem} />
      <ConfirmDialog open={deleteId !== null} title="Excluir Viatura" message="Tem certeza que deseja excluir esta viatura?" onConfirm={handleDelete} onCancel={() => setDeleteId(null)} />
    </>
  );
}
