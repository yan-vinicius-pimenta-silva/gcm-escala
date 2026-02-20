import { useState } from 'react';
import { DataGrid, type GridColDef } from '@mui/x-data-grid';
import { Box, IconButton } from '@mui/material';
import EditIcon from '@mui/icons-material/Edit';
import DeleteIcon from '@mui/icons-material/Delete';
import { useSnackbar } from 'notistack';
import PageHeader from '../../components/ui/PageHeader';
import ConfirmDialog from '../../components/ui/ConfirmDialog';
import EventoForm from './EventoForm';
import { useEventos, useCreateEvento, useUpdateEvento, useDeleteEvento } from './useEventos';
import type { Evento } from '../../types';
import type { EventoFormData } from './eventoSchema';
import { formatDateToDisplay } from '../../utils/date';

export default function EventosPage() {
  const { data: items = [], isLoading } = useEventos();
  const createMut = useCreateEvento();
  const updateMut = useUpdateEvento();
  const deleteMut = useDeleteEvento();
  const { enqueueSnackbar } = useSnackbar();
  const [formOpen, setFormOpen] = useState(false);
  const [editItem, setEditItem] = useState<Evento | null>(null);
  const [deleteId, setDeleteId] = useState<number | null>(null);

  const handleSubmit = async (data: EventoFormData) => {
    try {
      if (editItem) { await updateMut.mutateAsync({ id: editItem.id, data }); enqueueSnackbar('Evento atualizado', { variant: 'success' }); }
      else { await createMut.mutateAsync(data); enqueueSnackbar('Evento criado', { variant: 'success' }); }
      setFormOpen(false);
    } catch (err: any) { enqueueSnackbar(err.response?.data?.message || 'Erro ao salvar', { variant: 'error' }); }
  };

  const handleDelete = async () => {
    if (deleteId == null) return;
    try { await deleteMut.mutateAsync(deleteId); enqueueSnackbar('Evento excluído', { variant: 'success' }); }
    catch (err: any) { enqueueSnackbar(err.response?.data?.message || 'Erro ao excluir', { variant: 'error' }); }
    setDeleteId(null);
  };

  const columns: GridColDef[] = [
    { field: 'id', headerName: 'ID', width: 70 },
    { field: 'nome', headerName: 'Nome', flex: 1 },
    { field: 'dataInicio', headerName: 'Início', width: 130, valueFormatter: (value) => formatDateToDisplay(value) },
    { field: 'dataFim', headerName: 'Fim', width: 130, valueFormatter: (value) => formatDateToDisplay(value) },
    { field: 'actions', headerName: 'Ações', width: 120, sortable: false, renderCell: (p) => (<><IconButton size="small" onClick={() => { setEditItem(p.row); setFormOpen(true); }}><EditIcon /></IconButton><IconButton size="small" onClick={() => setDeleteId(p.row.id)}><DeleteIcon /></IconButton></>) },
  ];

  return (
    <>
      <PageHeader title="Eventos" onAdd={() => { setEditItem(null); setFormOpen(true); }} />
      <Box sx={{ width: '100%', overflowX: 'auto' }}>
        <DataGrid rows={items} columns={columns} loading={isLoading} autoHeight pageSizeOptions={[10, 25, 50]} initialState={{ pagination: { paginationModel: { pageSize: 10 } } }} />
      </Box>
      <EventoForm open={formOpen} onClose={() => setFormOpen(false)} onSubmit={handleSubmit} editData={editItem} />
      <ConfirmDialog open={deleteId !== null} title="Excluir Evento" message="Tem certeza que deseja excluir este evento?" onConfirm={handleDelete} onCancel={() => setDeleteId(null)} />
    </>
  );
}
