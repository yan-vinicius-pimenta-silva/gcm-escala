import apiClient from './client';
import type { Ferias } from '../types';
export interface CreateFeriasRequest { guardaId: number; dataInicio: string; dataFim: string; observacao?: string; }
export interface UpdateFeriasRequest { guardaId: number; dataInicio: string; dataFim: string; observacao?: string; }
export const getFeriasList = () => apiClient.get<Ferias[]>('/ferias').then(r => r.data);
export const createFerias = (data: CreateFeriasRequest) => apiClient.post<Ferias>('/ferias', data).then(r => r.data);
export const updateFerias = (id: number, data: UpdateFeriasRequest) => apiClient.put<Ferias>(`/ferias/${id}`, data).then(r => r.data);
export const deleteFerias = (id: number) => apiClient.delete(`/ferias/${id}`);
