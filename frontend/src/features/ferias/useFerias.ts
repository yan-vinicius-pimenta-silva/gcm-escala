import { useQuery, useMutation, useQueryClient } from '@tanstack/react-query';
import { getFeriasList, createFerias, updateFerias, deleteFerias } from '../../api/ferias';
import type { CreateFeriasRequest, UpdateFeriasRequest } from '../../api/ferias';
export function useFeriasList() { return useQuery({ queryKey: ['ferias'], queryFn: getFeriasList }); }
export function useCreateFerias() { const qc = useQueryClient(); return useMutation({ mutationFn: (d: CreateFeriasRequest) => createFerias(d), onSuccess: () => qc.invalidateQueries({ queryKey: ['ferias'] }) }); }
export function useUpdateFerias() { const qc = useQueryClient(); return useMutation({ mutationFn: ({ id, data }: { id: number; data: UpdateFeriasRequest }) => updateFerias(id, data), onSuccess: () => qc.invalidateQueries({ queryKey: ['ferias'] }) }); }
export function useDeleteFerias() { const qc = useQueryClient(); return useMutation({ mutationFn: (id: number) => deleteFerias(id), onSuccess: () => qc.invalidateQueries({ queryKey: ['ferias'] }) }); }
