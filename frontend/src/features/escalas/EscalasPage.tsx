import { useState } from 'react';
import { Box } from '@mui/material';
import { useSnackbar } from 'notistack';
import PageHeader from '../../components/ui/PageHeader';
import ConfirmDialog from '../../components/ui/ConfirmDialog';
import EscalaFilters from './EscalaFilters';
import EscalaItemForm from './EscalaItemForm';
import EscalaGrid from './EscalaGrid';
import EscalaValidationModal from './EscalaValidationModal';
import { useEscalas, useEscala, useCreateEscala, useAddEscalaItem, useUpdateEscalaItem, useDeleteEscalaItem, usePublicarEscala, useFecharEscala } from './useEscalas';
import { useSetores } from '../setores/useSetores';
import type { StatusEscala, EscalaItem, TipoSetor } from '../../types';
import type { ConflictError, AddEscalaItemRequest } from '../../api/escalas';

export default function EscalasPage() {
  const { enqueueSnackbar } = useSnackbar();
  const [setorId, setSetorId] = useState<number | null>(null);
  const [mes, setMes] = useState(new Date().getMonth() + 1);
  const [ano, setAno] = useState(new Date().getFullYear());
  const [quinzena, setQuinzena] = useState(1);
  const [escalaId, setEscalaId] = useState<number | null>(null);
  const [editingItem, setEditingItem] = useState<EscalaItem | null>(null);
  const [deleteItemId, setDeleteItemId] = useState<number | null>(null);
  const [validationErrors, setValidationErrors] = useState<ConflictError[]>([]);

  const { data: setores = [] } = useSetores();
  const { data: escalaDetail, isLoading: detailLoading } = useEscala(escalaId);
  const escalasQuery = useEscalas({ ano, mes, quinzena, setorId: setorId ?? undefined }, false);
  const createMut = useCreateEscala();
  const addItemMut = useAddEscalaItem();
  const updateItemMut = useUpdateEscalaItem();
  const deleteItemMut = useDeleteEscalaItem();
  const publicarMut = usePublicarEscala();
  const fecharMut = useFecharEscala();

  const setor = setores.find(s => s.id === setorId);
  const tipoSetor = setor?.tipo as TipoSetor | undefined;
  const status = escalaDetail?.status as StatusEscala | null ?? null;
  const isReadOnly = status !== null && status !== 'Rascunho';
  const items = escalaDetail?.itens || [];

  async function handleCarregar() {
    if (!setorId) return;
    try {
      const result = await escalasQuery.refetch();
      const list = result.data || [];
      if (list.length > 0) {
        setEscalaId(list[0].id);
        enqueueSnackbar('Escala carregada', { variant: 'success' });
      } else {
        const created = await createMut.mutateAsync({ ano, mes, quinzena, setorId });
        setEscalaId(created.id);
        enqueueSnackbar('Nova escala criada', { variant: 'success' });
      }
    } catch (err: any) {
      enqueueSnackbar(err.response?.data?.message || 'Erro ao carregar escala', { variant: 'error' });
    }
  }

  async function handleSubmitItems(requestItems: AddEscalaItemRequest[]) {
    if (!escalaId) return;
    const allErrors: ConflictError[] = [];
    for (const item of requestItems) {
      try {
        if (editingItem) {
          await updateItemMut.mutateAsync({ escalaId, itemId: editingItem.id, data: item });
        } else {
          await addItemMut.mutateAsync({ escalaId, data: item });
        }
      } catch (err: any) {
        const errs = err.response?.data?.errors || err.response?.data;
        if (Array.isArray(errs)) allErrors.push(...errs);
        else enqueueSnackbar(err.response?.data?.message || `Erro ao salvar item (${item.data})`, { variant: 'error' });
      }
    }
    if (allErrors.length > 0) {
      setValidationErrors(allErrors);
    } else {
      enqueueSnackbar(editingItem ? 'Item atualizado' : 'Itens adicionados', { variant: 'success' });
      setEditingItem(null);
    }
  }

  async function handleDeleteItem() {
    if (!escalaId || deleteItemId === null) return;
    try {
      await deleteItemMut.mutateAsync({ escalaId, itemId: deleteItemId });
      enqueueSnackbar('Item excluido', { variant: 'success' });
    } catch (err: any) {
      enqueueSnackbar(err.response?.data?.message || 'Erro ao excluir', { variant: 'error' });
    }
    setDeleteItemId(null);
  }

  async function handlePublicar() {
    if (!escalaId) return;
    try {
      await publicarMut.mutateAsync(escalaId);
      enqueueSnackbar('Escala publicada', { variant: 'success' });
    } catch (err: any) {
      enqueueSnackbar(err.response?.data?.message || 'Erro ao publicar', { variant: 'error' });
    }
  }

  async function handleFechar() {
    if (!escalaId) return;
    try {
      await fecharMut.mutateAsync(escalaId);
      enqueueSnackbar('Escala fechada', { variant: 'success' });
    } catch (err: any) {
      enqueueSnackbar(err.response?.data?.message || 'Erro ao fechar', { variant: 'error' });
    }
  }

  return (
    <Box>
      <PageHeader title="Escalas" />
      <EscalaFilters
        setorId={setorId} onSetorChange={(id) => { setSetorId(id); setEscalaId(null); }}
        mes={mes} onMesChange={(m) => { setMes(m); setEscalaId(null); }}
        ano={ano} onAnoChange={(a) => { setAno(a); setEscalaId(null); }}
        quinzena={quinzena} onQuinzenaChange={(q) => { setQuinzena(q); setEscalaId(null); }}
        onCarregar={handleCarregar}
        onPublicar={handlePublicar}
        onFechar={handleFechar}
        status={status}
        isLoading={escalasQuery.isFetching || createMut.isPending || detailLoading}
      />
      {escalaId && tipoSetor && (
        <Box display="flex" gap={3} mt={3} flexDirection={{ xs: 'column', md: 'row' }}>
          <Box flex={1} maxWidth={{ md: 420 }}>
            <EscalaItemForm
              tipoSetor={tipoSetor}
              ano={ano} mes={mes} quinzena={quinzena}
              editingItem={editingItem}
              onSubmit={handleSubmitItems}
              onCancelEdit={() => setEditingItem(null)}
              disabled={isReadOnly}
            />
          </Box>
          <Box flex={2}>
            <EscalaGrid
              items={items}
              onEdit={(item) => setEditingItem(item)}
              onDelete={(id) => setDeleteItemId(id)}
              isReadOnly={isReadOnly}
            />
          </Box>
        </Box>
      )}
      <EscalaValidationModal
        open={validationErrors.length > 0}
        errors={validationErrors}
        onClose={() => setValidationErrors([])}
      />
      <ConfirmDialog
        open={deleteItemId !== null}
        title="Excluir Item"
        message="Tem certeza que deseja excluir este lancamento?"
        onConfirm={handleDeleteItem}
        onCancel={() => setDeleteItemId(null)}
      />
    </Box>
  );
}
