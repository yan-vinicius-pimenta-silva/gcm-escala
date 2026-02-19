import { useState } from 'react';
import { DataGrid, type GridColDef } from '@mui/x-data-grid';
import { Box, IconButton } from '@mui/material';
import EditIcon from '@mui/icons-material/Edit';
import DeleteIcon from '@mui/icons-material/Delete';
import { useSnackbar } from 'notistack';
import PageHeader from '../../components/ui/PageHeader';
import ConfirmDialog from '../../components/ui/ConfirmDialog';
import RetForm from './RetForm';
import { useRets, useCreateRet, useUpdateRet, useDeleteRet } from './useRets';
import type { Ret } from '../../types';
import type { RetFormData } from './retSchema';
import type { TipoRet } from '../../types';
import { formatDateToDisplay } from '../../utils/date';

export default function RetsPage() {
  const { data: items = [], isLoading } = useRets();
  const createMut = useCreateRet();
  const updateMut = useUpdateRet();
  const deleteMut = useDeleteRet();
  const { enqueueSnackbar } = useSnackbar();
  const [formOpen, setFormOpen] = useState(false);
  const [editItem, setEditItem] = useState<Ret | null>(null);
  const [deleteId, setDeleteId] = useState<number | null>(null);

  const handleSubmit = async (data: RetFormData) => {
    try {
      const payload = { ...data, tipo: data.tipo as TipoRet };
      if (editItem) { await updateMut.mutateAsync({ id: editItem.id, data: payload }); enqueueSnackbar('RET atualizado', { variant: 'success' }); }
      else { await createMut.mutateAsync(payload); enqueueSnackbar('RET criado', { variant: 'success' }); }
      setFormOpen(false);
    } catch (err: any) { enqueueSnackbar(err.response?.data?.message || 'Erro ao salvar', { variant: 'error' }); }
  };

  const handleDelete = async () => {
    if (deleteId == null) return;
    try { await deleteMut.mutateAsync(deleteId); enqueueSnackbar('RET excluído', { variant: 'success' }); }
    catch (err: any) { enqueueSnackbar(err.response?.data?.message || 'Erro ao excluir', { variant: 'error' }); }
    setDeleteId(null);
  };

  const columns: GridColDef[] = [
    { field: 'id', headerName: 'ID', width: 70 },
    { field: 'guardaNome', headerName: 'Guarda', flex: 1 },
    { field: 'data', headerName: 'Data', width: 130, valueFormatter: (value) => formatDateToDisplay(value) },
    { field: 'horarioInicio', headerName: 'Início', width: 100 },
    { field: 'horarioFim', headerName: 'Fim', width: 100 },
    { field: 'tipo', headerName: 'Tipo', width: 120 },
    { field: 'eventoNome', headerName: 'Evento', flex: 1 },
    { field: 'observacao', headerName: 'Observação', flex: 1 },
    { field: 'actions', headerName: 'Ações', width: 120, sortable: false, renderCell: (p) => (<><IconButton size="small" onClick={() => { setEditItem(p.row); setFormOpen(true); }}><EditIcon /></IconButton><IconButton size="small" onClick={() => setDeleteId(p.row.id)}><DeleteIcon /></IconButton></>) },
  ];

  return (
    <>
      <PageHeader title="RETs" onAdd={() => { setEditItem(null); setFormOpen(true); }} />
      <Box sx={{ width: '100%', overflowX: 'auto' }}>
        <DataGrid rows={items} columns={columns} loading={isLoading} autoHeight pageSizeOptions={[10, 25, 50]} initialState={{ pagination: { paginationModel: { pageSize: 10 } } }} />
      </Box>
      <RetForm open={formOpen} onClose={() => setFormOpen(false)} onSubmit={handleSubmit} editData={editItem} />
      <ConfirmDialog open={deleteId !== null} title="Excluir RET" message="Tem certeza que deseja excluir este RET?" onConfirm={handleDelete} onCancel={() => setDeleteId(null)} />
    </>
  );
}
