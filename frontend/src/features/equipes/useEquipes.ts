import { useQuery, useMutation, useQueryClient } from '@tanstack/react-query';
import { getEquipes, createEquipe, updateEquipe, deleteEquipe } from '../../api/equipes';
import type { CreateEquipeRequest, UpdateEquipeRequest } from '../../api/equipes';
export function useEquipes() { return useQuery({ queryKey: ['equipes'], queryFn: getEquipes }); }
export function useCreateEquipe() { const qc = useQueryClient(); return useMutation({ mutationFn: (d: CreateEquipeRequest) => createEquipe(d), onSuccess: () => qc.invalidateQueries({ queryKey: ['equipes'] }) }); }
export function useUpdateEquipe() { const qc = useQueryClient(); return useMutation({ mutationFn: ({ id, data }: { id: number; data: UpdateEquipeRequest }) => updateEquipe(id, data), onSuccess: () => qc.invalidateQueries({ queryKey: ['equipes'] }) }); }
export function useDeleteEquipe() { const qc = useQueryClient(); return useMutation({ mutationFn: (id: number) => deleteEquipe(id), onSuccess: () => qc.invalidateQueries({ queryKey: ['equipes'] }) }); }
