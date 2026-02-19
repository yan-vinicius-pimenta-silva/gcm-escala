import { useState } from 'react';
import { DataGrid, type GridColDef } from '@mui/x-data-grid';
import { Box, Chip, IconButton } from '@mui/material';
import EditIcon from '@mui/icons-material/Edit';
import DeleteIcon from '@mui/icons-material/Delete';
import { useSnackbar } from 'notistack';
import PageHeader from '../../components/ui/PageHeader';
import ConfirmDialog from '../../components/ui/ConfirmDialog';
import SetorForm from './SetorForm';
import { useSetores, useCreateSetor, useUpdateSetor, useDeleteSetor } from './useSetores';
import type { Setor } from '../../types';
import type { SetorFormData } from './setorSchema';

export default function SetoresPage() {
  const { data: setores = [], isLoading } = useSetores();
  const createMutation = useCreateSetor();
  const updateMutation = useUpdateSetor();
  const deleteMutation = useDeleteSetor();
  const { enqueueSnackbar } = useSnackbar();

  const [formOpen, setFormOpen] = useState(false);
  const [editItem, setEditItem] = useState<Setor | null>(null);
  const [deleteId, setDeleteId] = useState<number | null>(null);

  const handleAdd = () => { setEditItem(null); setFormOpen(true); };
  const handleEdit = (item: Setor) => { setEditItem(item); setFormOpen(true); };

  const handleSubmit = async (data: SetorFormData) => {
    try {
      if (editItem) {
        await updateMutation.mutateAsync({ id: editItem.id, data });
        enqueueSnackbar('Setor atualizado', { variant: 'success' });
      } else {
        await createMutation.mutateAsync(data);
        enqueueSnackbar('Setor criado', { variant: 'success' });
      }
      setFormOpen(false);
    } catch (err: any) {
      enqueueSnackbar(err.response?.data?.message || 'Erro ao salvar', { variant: 'error' });
    }
  };

  const handleDelete = async () => {
    if (deleteId == null) return;
    try {
      await deleteMutation.mutateAsync(deleteId);
      enqueueSnackbar('Setor excluído', { variant: 'success' });
    } catch (err: any) {
      enqueueSnackbar(err.response?.data?.message || 'Erro ao excluir', { variant: 'error' });
    }
    setDeleteId(null);
  };

  const columns: GridColDef[] = [
    { field: 'id', headerName: 'ID', width: 70 },
    { field: 'nome', headerName: 'Nome', flex: 1 },
    { field: 'tipo', headerName: 'Tipo', width: 180 },
    {
      field: 'ativo', headerName: 'Status', width: 100,
      renderCell: (params) => (
        <Chip label={params.value ? 'Ativo' : 'Inativo'} color={params.value ? 'success' : 'default'} size="small" />
      ),
    },
    {
      field: 'actions', headerName: 'Ações', width: 120, sortable: false,
      renderCell: (params) => (
        <>
          <IconButton size="small" onClick={() => handleEdit(params.row)}><EditIcon /></IconButton>
          <IconButton size="small" onClick={() => setDeleteId(params.row.id)}><DeleteIcon /></IconButton>
        </>
      ),
    },
  ];

  return (
    <>
      <PageHeader title="Setores" onAdd={handleAdd} />
      <Box sx={{ width: '100%', overflowX: 'auto' }}>
        <DataGrid rows={setores} columns={columns} loading={isLoading} autoHeight
          pageSizeOptions={[10, 25, 50]} initialState={{ pagination: { paginationModel: { pageSize: 10 } } }}
        />
      </Box>
      <SetorForm open={formOpen} onClose={() => setFormOpen(false)} onSubmit={handleSubmit} editData={editItem} />
      <ConfirmDialog
        open={deleteId !== null} title="Excluir Setor"
        message="Tem certeza que deseja excluir este setor?"
        onConfirm={handleDelete} onCancel={() => setDeleteId(null)}
      />
    </>
  );
}
