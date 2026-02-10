import { useQuery, useMutation, useQueryClient } from '@tanstack/react-query';
import { getGuardas, createGuarda, updateGuarda, deleteGuarda } from '../../api/guardas';
import type { CreateGuardaRequest, UpdateGuardaRequest } from '../../api/guardas';

export function useGuardas() {
  return useQuery({ queryKey: ['guardas'], queryFn: getGuardas });
}
export function useCreateGuarda() {
  const qc = useQueryClient();
  return useMutation({ mutationFn: (data: CreateGuardaRequest) => createGuarda(data), onSuccess: () => qc.invalidateQueries({ queryKey: ['guardas'] }) });
}
export function useUpdateGuarda() {
  const qc = useQueryClient();
  return useMutation({ mutationFn: ({ id, data }: { id: number; data: UpdateGuardaRequest }) => updateGuarda(id, data), onSuccess: () => qc.invalidateQueries({ queryKey: ['guardas'] }) });
}
export function useDeleteGuarda() {
  const qc = useQueryClient();
  return useMutation({ mutationFn: (id: number) => deleteGuarda(id), onSuccess: () => qc.invalidateQueries({ queryKey: ['guardas'] }) });
}
