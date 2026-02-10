import { useQuery, useMutation, useQueryClient } from '@tanstack/react-query';
import {
  getEscalas, getEscala, createEscala, addEscalaItem,
  updateEscalaItem, deleteEscalaItem, publicarEscala,
  fecharEscala, deleteEscala,
} from '../../api/escalas';
import type { EscalaFiltersParams, CreateEscalaRequest, AddEscalaItemRequest, UpdateEscalaItemRequest } from '../../api/escalas';

export function useEscalas(params: EscalaFiltersParams, enabled: boolean) {
  return useQuery({ queryKey: ['escalas', params], queryFn: () => getEscalas(params), enabled });
}

export function useEscala(id: number | null) {
  return useQuery({ queryKey: ['escala', id], queryFn: () => getEscala(id!), enabled: id !== null && id > 0 });
}

export function useCreateEscala() {
  const qc = useQueryClient();
  return useMutation({ mutationFn: (data: CreateEscalaRequest) => createEscala(data), onSuccess: () => qc.invalidateQueries({ queryKey: ['escalas'] }) });
}

export function useAddEscalaItem() {
  const qc = useQueryClient();
  return useMutation({
    mutationFn: ({ escalaId, data }: { escalaId: number; data: AddEscalaItemRequest }) => addEscalaItem(escalaId, data),
    onSuccess: (_, vars) => qc.invalidateQueries({ queryKey: ['escala', vars.escalaId] }),
  });
}

export function useUpdateEscalaItem() {
  const qc = useQueryClient();
  return useMutation({
    mutationFn: ({ escalaId, itemId, data }: { escalaId: number; itemId: number; data: UpdateEscalaItemRequest }) => updateEscalaItem(escalaId, itemId, data),
    onSuccess: (_, vars) => qc.invalidateQueries({ queryKey: ['escala', vars.escalaId] }),
  });
}

export function useDeleteEscalaItem() {
  const qc = useQueryClient();
  return useMutation({
    mutationFn: ({ escalaId, itemId }: { escalaId: number; itemId: number }) => deleteEscalaItem(escalaId, itemId),
    onSuccess: (_, vars) => qc.invalidateQueries({ queryKey: ['escala', vars.escalaId] }),
  });
}

export function usePublicarEscala() {
  const qc = useQueryClient();
  return useMutation({
    mutationFn: (id: number) => publicarEscala(id),
    onSuccess: () => { qc.invalidateQueries({ queryKey: ['escalas'] }); qc.invalidateQueries({ queryKey: ['escala'] }); },
  });
}

export function useFecharEscala() {
  const qc = useQueryClient();
  return useMutation({
    mutationFn: (id: number) => fecharEscala(id),
    onSuccess: () => { qc.invalidateQueries({ queryKey: ['escalas'] }); qc.invalidateQueries({ queryKey: ['escala'] }); },
  });
}

export function useDeleteEscala() {
  const qc = useQueryClient();
  return useMutation({ mutationFn: (id: number) => deleteEscala(id), onSuccess: () => qc.invalidateQueries({ queryKey: ['escalas'] }) });
}
