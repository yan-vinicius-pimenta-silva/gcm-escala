import { useState } from 'react';
import { DataGrid, type GridColDef } from '@mui/x-data-grid';
import { Chip, IconButton } from '@mui/material';
import EditIcon from '@mui/icons-material/Edit';
import DeleteIcon from '@mui/icons-material/Delete';
import { useSnackbar } from 'notistack';
import PageHeader from '../../components/ui/PageHeader';
import ConfirmDialog from '../../components/ui/ConfirmDialog';
import EquipeForm from './EquipeForm';
import { useEquipes, useCreateEquipe, useUpdateEquipe, useDeleteEquipe } from './useEquipes';
import type { Equipe } from '../../types';
import type { EquipeFormData } from './equipeSchema';

export default function EquipesPage() {
  const { data: items = [], isLoading } = useEquipes();
  const createMut = useCreateEquipe();
  const updateMut = useUpdateEquipe();
  const deleteMut = useDeleteEquipe();
  const { enqueueSnackbar } = useSnackbar();
  const [formOpen, setFormOpen] = useState(false);
  const [editItem, setEditItem] = useState<Equipe | null>(null);
  const [deleteId, setDeleteId] = useState<number | null>(null);

  const handleSubmit = async (data: EquipeFormData) => {
    try {
      if (editItem) { await updateMut.mutateAsync({ id: editItem.id, data }); enqueueSnackbar('Equipe atualizada', { variant: 'success' }); }
      else { await createMut.mutateAsync(data); enqueueSnackbar('Equipe criada', { variant: 'success' }); }
      setFormOpen(false);
    } catch (err: any) { enqueueSnackbar(err.response?.data?.message || 'Erro ao salvar', { variant: 'error' }); }
  };

  const handleDelete = async () => {
    if (deleteId == null) return;
    try { await deleteMut.mutateAsync(deleteId); enqueueSnackbar('Equipe excluída', { variant: 'success' }); }
    catch (err: any) { enqueueSnackbar(err.response?.data?.message || 'Erro ao excluir', { variant: 'error' }); }
    setDeleteId(null);
  };

  const columns: GridColDef[] = [
    { field: 'id', headerName: 'ID', width: 70 },
    { field: 'nome', headerName: 'Nome', flex: 1 },
    { field: 'membros', headerName: 'Membros', width: 300, renderCell: (p) => p.value?.map((m: any) => m.guardaNome).join(', ') || '' },
    { field: 'ativo', headerName: 'Status', width: 100, renderCell: (p) => <Chip label={p.value ? 'Ativo' : 'Inativo'} color={p.value ? 'success' : 'default'} size="small" /> },
    { field: 'actions', headerName: 'Ações', width: 120, sortable: false, renderCell: (p) => (<><IconButton size="small" onClick={() => { setEditItem(p.row); setFormOpen(true); }}><EditIcon /></IconButton><IconButton size="small" onClick={() => setDeleteId(p.row.id)}><DeleteIcon /></IconButton></>) },
  ];

  return (
    <>
      <PageHeader title="Equipes" onAdd={() => { setEditItem(null); setFormOpen(true); }} />
      <DataGrid rows={items} columns={columns} loading={isLoading} autoHeight pageSizeOptions={[10, 25, 50]} initialState={{ pagination: { paginationModel: { pageSize: 10 } } }} />
      <EquipeForm open={formOpen} onClose={() => setFormOpen(false)} onSubmit={handleSubmit} editData={editItem} />
      <ConfirmDialog open={deleteId !== null} title="Excluir Equipe" message="Tem certeza que deseja excluir esta equipe?" onConfirm={handleDelete} onCancel={() => setDeleteId(null)} />
    </>
  );
}
