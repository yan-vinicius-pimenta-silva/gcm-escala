import { useState } from 'react';
import { DataGrid, type GridColDef } from '@mui/x-data-grid';
import { Box, Chip, IconButton } from '@mui/material';
import EditIcon from '@mui/icons-material/Edit';
import DeleteIcon from '@mui/icons-material/Delete';
import { useSnackbar } from 'notistack';
import PageHeader from '../../components/ui/PageHeader';
import ConfirmDialog from '../../components/ui/ConfirmDialog';
import TurnoForm from './TurnoForm';
import { useTurnos, useCreateTurno, useUpdateTurno, useDeleteTurno } from './useTurnos';
import type { Turno } from '../../types';
import type { TurnoFormData } from './turnoSchema';

export default function TurnosPage() {
  const { data: items = [], isLoading } = useTurnos();
  const createMut = useCreateTurno();
  const updateMut = useUpdateTurno();
  const deleteMut = useDeleteTurno();
  const { enqueueSnackbar } = useSnackbar();
  const [formOpen, setFormOpen] = useState(false);
  const [editItem, setEditItem] = useState<Turno | null>(null);
  const [deleteId, setDeleteId] = useState<number | null>(null);

  const handleSubmit = async (data: TurnoFormData) => {
    try {
      if (editItem) { await updateMut.mutateAsync({ id: editItem.id, data }); enqueueSnackbar('Turno atualizado', { variant: 'success' }); }
      else { await createMut.mutateAsync(data); enqueueSnackbar('Turno criado', { variant: 'success' }); }
      setFormOpen(false);
    } catch (err: any) { enqueueSnackbar(err.response?.data?.message || 'Erro ao salvar', { variant: 'error' }); }
  };

  const handleDelete = async () => {
    if (deleteId == null) return;
    try { await deleteMut.mutateAsync(deleteId); enqueueSnackbar('Turno excluído', { variant: 'success' }); }
    catch (err: any) { enqueueSnackbar(err.response?.data?.message || 'Erro ao excluir', { variant: 'error' }); }
    setDeleteId(null);
  };

  const columns: GridColDef[] = [
    { field: 'id', headerName: 'ID', width: 70 },
    { field: 'nome', headerName: 'Nome', flex: 1 },
    { field: 'ativo', headerName: 'Status', width: 100, renderCell: (p) => <Chip label={p.value ? 'Ativo' : 'Inativo'} color={p.value ? 'success' : 'default'} size="small" /> },
    { field: 'actions', headerName: 'Ações', width: 120, sortable: false, renderCell: (p) => (<><IconButton size="small" onClick={() => { setEditItem(p.row); setFormOpen(true); }}><EditIcon /></IconButton><IconButton size="small" onClick={() => setDeleteId(p.row.id)}><DeleteIcon /></IconButton></>) },
  ];

  return (
    <>
      <PageHeader title="Turnos" onAdd={() => { setEditItem(null); setFormOpen(true); }} />
      <Box sx={{ width: '100%', overflowX: 'auto' }}>
        <DataGrid rows={items} columns={columns} loading={isLoading} autoHeight pageSizeOptions={[10, 25, 50]} initialState={{ pagination: { paginationModel: { pageSize: 10 } } }} />
      </Box>
      <TurnoForm open={formOpen} onClose={() => setFormOpen(false)} onSubmit={handleSubmit} editData={editItem} />
      <ConfirmDialog open={deleteId !== null} title="Excluir Turno" message="Tem certeza que deseja excluir este turno?" onConfirm={handleDelete} onCancel={() => setDeleteId(null)} />
    </>
  );
}
