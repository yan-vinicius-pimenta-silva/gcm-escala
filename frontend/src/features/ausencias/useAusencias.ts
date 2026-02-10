import { useQuery, useMutation, useQueryClient } from '@tanstack/react-query';
import { getAusencias, createAusencia, updateAusencia, deleteAusencia } from '../../api/ausencias';
import type { CreateAusenciaRequest, UpdateAusenciaRequest } from '../../api/ausencias';
export function useAusencias() { return useQuery({ queryKey: ['ausencias'], queryFn: getAusencias }); }
export function useCreateAusencia() { const qc = useQueryClient(); return useMutation({ mutationFn: (d: CreateAusenciaRequest) => createAusencia(d), onSuccess: () => qc.invalidateQueries({ queryKey: ['ausencias'] }) }); }
export function useUpdateAusencia() { const qc = useQueryClient(); return useMutation({ mutationFn: ({ id, data }: { id: number; data: UpdateAusenciaRequest }) => updateAusencia(id, data), onSuccess: () => qc.invalidateQueries({ queryKey: ['ausencias'] }) }); }
export function useDeleteAusencia() { const qc = useQueryClient(); return useMutation({ mutationFn: (id: number) => deleteAusencia(id), onSuccess: () => qc.invalidateQueries({ queryKey: ['ausencias'] }) }); }
