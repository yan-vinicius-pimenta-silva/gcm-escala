import apiClient from './client';
import type { Guarda } from '../types';

export interface CreateGuardaRequest { nome: string; telefone?: string; posicaoId: number; ativo: boolean; }
export interface UpdateGuardaRequest { nome: string; telefone?: string; posicaoId: number; ativo: boolean; }

export const getGuardas = () => apiClient.get<Guarda[]>('/guardas').then(r => r.data);
export const getGuarda = (id: number) => apiClient.get<Guarda>(`/guardas/${id}`).then(r => r.data);
export const createGuarda = (data: CreateGuardaRequest) => apiClient.post<Guarda>('/guardas', data).then(r => r.data);
export const updateGuarda = (id: number, data: UpdateGuardaRequest) => apiClient.put<Guarda>(`/guardas/${id}`, data).then(r => r.data);
export const deleteGuarda = (id: number) => apiClient.delete(`/guardas/${id}`);
