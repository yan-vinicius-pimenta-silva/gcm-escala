import { useState } from 'react';
import { DataGrid, type GridColDef } from '@mui/x-data-grid';
import { Box, Chip, IconButton, Typography } from '@mui/material';
import EditIcon from '@mui/icons-material/Edit';
import DeleteIcon from '@mui/icons-material/Delete';
import { useSnackbar } from 'notistack';
import PageHeader from '../../components/ui/PageHeader';
import ConfirmDialog from '../../components/ui/ConfirmDialog';
import HorarioForm from './HorarioForm';
import { useHorarios, useCreateHorario, useUpdateHorario, useDeleteHorario } from './useHorarios';
import type { Horario } from '../../types';
import type { HorarioFormData } from './horarioSchema';

export default function HorariosPage() {
  const { data: items = [], isLoading } = useHorarios();
  const createMut = useCreateHorario();
  const updateMut = useUpdateHorario();
  const deleteMut = useDeleteHorario();
  const { enqueueSnackbar } = useSnackbar();
  const [formOpen, setFormOpen] = useState(false);
  const [editItem, setEditItem] = useState<Horario | null>(null);
  const [deleteId, setDeleteId] = useState<number | null>(null);

  const handleSubmit = async (data: HorarioFormData) => {
    try {
      if (editItem) { await updateMut.mutateAsync({ id: editItem.id, data }); enqueueSnackbar('Horário atualizado', { variant: 'success' }); }
      else { await createMut.mutateAsync(data); enqueueSnackbar('Horário criado', { variant: 'success' }); }
      setFormOpen(false);
    } catch (err: any) { enqueueSnackbar(err.response?.data?.message || 'Erro ao salvar', { variant: 'error' }); }
  };

  const handleDelete = async () => {
    if (deleteId == null) return;
    try { await deleteMut.mutateAsync(deleteId); enqueueSnackbar('Horário excluído', { variant: 'success' }); }
    catch (err: any) { enqueueSnackbar(err.response?.data?.message || 'Erro ao excluir', { variant: 'error' }); }
    setDeleteId(null);
  };

  const columns: GridColDef[] = [
    { field: 'id', headerName: 'ID', width: 70 },
    { field: 'inicio', headerName: 'Início', width: 100 },
    { field: 'fim', headerName: 'Fim', width: 100 },
    {
      field: 'descricao', headerName: 'Descrição', flex: 1,
      renderCell: (p) => {
        const viraDia = p.row.fim < p.row.inicio;
        return <>{p.value}{viraDia && <Typography variant="caption" color="warning.main" sx={{ ml: 1 }}>(vira o dia)</Typography>}</>;
      },
    },
    { field: 'ativo', headerName: 'Status', width: 100, renderCell: (p) => <Chip label={p.value ? 'Ativo' : 'Inativo'} color={p.value ? 'success' : 'default'} size="small" /> },
    { field: 'actions', headerName: 'Ações', width: 120, sortable: false, renderCell: (p) => (<><IconButton size="small" onClick={() => { setEditItem(p.row); setFormOpen(true); }}><EditIcon /></IconButton><IconButton size="small" onClick={() => setDeleteId(p.row.id)}><DeleteIcon /></IconButton></>) },
  ];

  return (
    <>
      <PageHeader title="Horários" onAdd={() => { setEditItem(null); setFormOpen(true); }} />
      <Box sx={{ width: '100%', overflowX: 'auto' }}>
        <DataGrid rows={items} columns={columns} loading={isLoading} autoHeight pageSizeOptions={[10, 25, 50]} initialState={{ pagination: { paginationModel: { pageSize: 10 } } }} />
      </Box>
      <HorarioForm open={formOpen} onClose={() => setFormOpen(false)} onSubmit={handleSubmit} editData={editItem} />
      <ConfirmDialog open={deleteId !== null} title="Excluir Horário" message="Tem certeza que deseja excluir este horário?" onConfirm={handleDelete} onCancel={() => setDeleteId(null)} />
    </>
  );
}
