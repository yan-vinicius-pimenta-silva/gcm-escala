import { useQuery, useMutation, useQueryClient } from '@tanstack/react-query';
import { getEventos, createEvento, updateEvento, deleteEvento } from '../../api/eventos';
import type { CreateEventoRequest, UpdateEventoRequest } from '../../api/eventos';
export function useEventos() { return useQuery({ queryKey: ['eventos'], queryFn: getEventos }); }
export function useCreateEvento() { const qc = useQueryClient(); return useMutation({ mutationFn: (d: CreateEventoRequest) => createEvento(d), onSuccess: () => qc.invalidateQueries({ queryKey: ['eventos'] }) }); }
export function useUpdateEvento() { const qc = useQueryClient(); return useMutation({ mutationFn: ({ id, data }: { id: number; data: UpdateEventoRequest }) => updateEvento(id, data), onSuccess: () => qc.invalidateQueries({ queryKey: ['eventos'] }) }); }
export function useDeleteEvento() { const qc = useQueryClient(); return useMutation({ mutationFn: (id: number) => deleteEvento(id), onSuccess: () => qc.invalidateQueries({ queryKey: ['eventos'] }) }); }
