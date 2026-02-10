import { useQuery, useMutation, useQueryClient } from '@tanstack/react-query';
import { getPosicoes, createPosicao, updatePosicao, deletePosicao } from '../../api/posicoes';
import type { CreatePosicaoRequest, UpdatePosicaoRequest } from '../../api/posicoes';

export function usePosicoes() {
  return useQuery({ queryKey: ['posicoes'], queryFn: getPosicoes });
}
export function useCreatePosicao() {
  const qc = useQueryClient();
  return useMutation({ mutationFn: (data: CreatePosicaoRequest) => createPosicao(data), onSuccess: () => qc.invalidateQueries({ queryKey: ['posicoes'] }) });
}
export function useUpdatePosicao() {
  const qc = useQueryClient();
  return useMutation({ mutationFn: ({ id, data }: { id: number; data: UpdatePosicaoRequest }) => updatePosicao(id, data), onSuccess: () => qc.invalidateQueries({ queryKey: ['posicoes'] }) });
}
export function useDeletePosicao() {
  const qc = useQueryClient();
  return useMutation({ mutationFn: (id: number) => deletePosicao(id), onSuccess: () => qc.invalidateQueries({ queryKey: ['posicoes'] }) });
}
