import { useQuery, useMutation, useQueryClient } from '@tanstack/react-query';
import { getViaturas, createViatura, updateViatura, deleteViatura } from '../../api/viaturas';
import type { CreateViaturaRequest, UpdateViaturaRequest } from '../../api/viaturas';
export function useViaturas() { return useQuery({ queryKey: ['viaturas'], queryFn: getViaturas }); }
export function useCreateViatura() { const qc = useQueryClient(); return useMutation({ mutationFn: (d: CreateViaturaRequest) => createViatura(d), onSuccess: () => qc.invalidateQueries({ queryKey: ['viaturas'] }) }); }
export function useUpdateViatura() { const qc = useQueryClient(); return useMutation({ mutationFn: ({ id, data }: { id: number; data: UpdateViaturaRequest }) => updateViatura(id, data), onSuccess: () => qc.invalidateQueries({ queryKey: ['viaturas'] }) }); }
export function useDeleteViatura() { const qc = useQueryClient(); return useMutation({ mutationFn: (id: number) => deleteViatura(id), onSuccess: () => qc.invalidateQueries({ queryKey: ['viaturas'] }) }); }
