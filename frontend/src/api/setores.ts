import apiClient from './client';
import type { Setor } from '../types';

export interface CreateSetorRequest { nome: string; tipo: string; ativo: boolean; }
export interface UpdateSetorRequest { nome: string; tipo: string; ativo: boolean; }

export const getSetores = () => apiClient.get<Setor[]>('/setores').then(r => r.data);
export const getSetor = (id: number) => apiClient.get<Setor>(`/setores/${id}`).then(r => r.data);
export const createSetor = (data: CreateSetorRequest) => apiClient.post<Setor>('/setores', data).then(r => r.data);
export const updateSetor = (id: number, data: UpdateSetorRequest) => apiClient.put<Setor>(`/setores/${id}`, data).then(r => r.data);
export const deleteSetor = (id: number) => apiClient.delete(`/setores/${id}`);
