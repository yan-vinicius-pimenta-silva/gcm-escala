import apiClient from './client';
import type { Ausencia } from '../types';
export interface CreateAusenciaRequest { guardaId: number; dataInicio: string; dataFim: string; motivo: string; observacoes?: string; }
export interface UpdateAusenciaRequest { guardaId: number; dataInicio: string; dataFim: string; motivo: string; observacoes?: string; }
export const getAusencias = () => apiClient.get<Ausencia[]>('/ausencias').then(r => r.data);
export const createAusencia = (data: CreateAusenciaRequest) => apiClient.post<Ausencia>('/ausencias', data).then(r => r.data);
export const updateAusencia = (id: number, data: UpdateAusenciaRequest) => apiClient.put<Ausencia>(`/ausencias/${id}`, data).then(r => r.data);
export const deleteAusencia = (id: number) => apiClient.delete(`/ausencias/${id}`);
