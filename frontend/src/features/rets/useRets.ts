import { useQuery, useMutation, useQueryClient } from '@tanstack/react-query';
import { getRets, createRet, updateRet, deleteRet } from '../../api/rets';
import type { CreateRetRequest, UpdateRetRequest } from '../../api/rets';
export function useRets(params?: { guardaId?: number; mes?: number; ano?: number }) { return useQuery({ queryKey: ['rets', params], queryFn: () => getRets(params) }); }
export function useCreateRet() { const qc = useQueryClient(); return useMutation({ mutationFn: (d: CreateRetRequest) => createRet(d), onSuccess: () => qc.invalidateQueries({ queryKey: ['rets'] }) }); }
export function useUpdateRet() { const qc = useQueryClient(); return useMutation({ mutationFn: ({ id, data }: { id: number; data: UpdateRetRequest }) => updateRet(id, data), onSuccess: () => qc.invalidateQueries({ queryKey: ['rets'] }) }); }
export function useDeleteRet() { const qc = useQueryClient(); return useMutation({ mutationFn: (id: number) => deleteRet(id), onSuccess: () => qc.invalidateQueries({ queryKey: ['rets'] }) }); }
