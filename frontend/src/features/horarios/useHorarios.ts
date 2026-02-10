import { useQuery, useMutation, useQueryClient } from '@tanstack/react-query';
import { getHorarios, createHorario, updateHorario, deleteHorario } from '../../api/horarios';
import type { CreateHorarioRequest, UpdateHorarioRequest } from '../../api/horarios';

export function useHorarios() {
  return useQuery({ queryKey: ['horarios'], queryFn: getHorarios });
}
export function useCreateHorario() {
  const qc = useQueryClient();
  return useMutation({ mutationFn: (data: CreateHorarioRequest) => createHorario(data), onSuccess: () => qc.invalidateQueries({ queryKey: ['horarios'] }) });
}
export function useUpdateHorario() {
  const qc = useQueryClient();
  return useMutation({ mutationFn: ({ id, data }: { id: number; data: UpdateHorarioRequest }) => updateHorario(id, data), onSuccess: () => qc.invalidateQueries({ queryKey: ['horarios'] }) });
}
export function useDeleteHorario() {
  const qc = useQueryClient();
  return useMutation({ mutationFn: (id: number) => deleteHorario(id), onSuccess: () => qc.invalidateQueries({ queryKey: ['horarios'] }) });
}
