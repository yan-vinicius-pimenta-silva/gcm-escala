import { useQuery, useMutation, useQueryClient } from '@tanstack/react-query';
import { getTurnos, createTurno, updateTurno, deleteTurno } from '../../api/turnos';
import type { CreateTurnoRequest, UpdateTurnoRequest } from '../../api/turnos';

export function useTurnos() {
  return useQuery({ queryKey: ['turnos'], queryFn: getTurnos });
}
export function useCreateTurno() {
  const qc = useQueryClient();
  return useMutation({ mutationFn: (data: CreateTurnoRequest) => createTurno(data), onSuccess: () => qc.invalidateQueries({ queryKey: ['turnos'] }) });
}
export function useUpdateTurno() {
  const qc = useQueryClient();
  return useMutation({ mutationFn: ({ id, data }: { id: number; data: UpdateTurnoRequest }) => updateTurno(id, data), onSuccess: () => qc.invalidateQueries({ queryKey: ['turnos'] }) });
}
export function useDeleteTurno() {
  const qc = useQueryClient();
  return useMutation({ mutationFn: (id: number) => deleteTurno(id), onSuccess: () => qc.invalidateQueries({ queryKey: ['turnos'] }) });
}
