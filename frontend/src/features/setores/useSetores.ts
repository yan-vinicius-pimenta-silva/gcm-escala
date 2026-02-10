import { useQuery, useMutation, useQueryClient } from '@tanstack/react-query';
import { getSetores, createSetor, updateSetor, deleteSetor } from '../../api/setores';
import type { CreateSetorRequest, UpdateSetorRequest } from '../../api/setores';

export function useSetores() {
  return useQuery({ queryKey: ['setores'], queryFn: getSetores });
}

export function useCreateSetor() {
  const qc = useQueryClient();
  return useMutation({
    mutationFn: (data: CreateSetorRequest) => createSetor(data),
    onSuccess: () => qc.invalidateQueries({ queryKey: ['setores'] }),
  });
}

export function useUpdateSetor() {
  const qc = useQueryClient();
  return useMutation({
    mutationFn: ({ id, data }: { id: number; data: UpdateSetorRequest }) => updateSetor(id, data),
    onSuccess: () => qc.invalidateQueries({ queryKey: ['setores'] }),
  });
}

export function useDeleteSetor() {
  const qc = useQueryClient();
  return useMutation({
    mutationFn: (id: number) => deleteSetor(id),
    onSuccess: () => qc.invalidateQueries({ queryKey: ['setores'] }),
  });
}
