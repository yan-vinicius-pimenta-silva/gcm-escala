import apiClient from './client';
import type { Ret } from '../types';
import type { TipoRet } from '../types';
export interface CreateRetRequest { guardaId: number; data: string; horarioInicio: string; tipo: TipoRet; eventoId?: number; observacao?: string; }
export interface UpdateRetRequest { guardaId: number; data: string; horarioInicio: string; tipo: TipoRet; eventoId?: number; observacao?: string; }
export const getRets = (params?: { guardaId?: number; mes?: number; ano?: number }) => apiClient.get<Ret[]>('/rets', { params }).then(r => r.data);
export const createRet = (data: CreateRetRequest) => apiClient.post<Ret>('/rets', data).then(r => r.data);
export const updateRet = (id: number, data: UpdateRetRequest) => apiClient.put<Ret>(`/rets/${id}`, data).then(r => r.data);
export const deleteRet = (id: number) => apiClient.delete(`/rets/${id}`);
