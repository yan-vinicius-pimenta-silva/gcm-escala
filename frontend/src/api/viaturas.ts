import apiClient from './client';
import type { Viatura } from '../types';
export interface CreateViaturaRequest { identificador: string; ativo: boolean; }
export interface UpdateViaturaRequest { identificador: string; ativo: boolean; }
export const getViaturas = () => apiClient.get<Viatura[]>('/viaturas').then(r => r.data);
export const createViatura = (data: CreateViaturaRequest) => apiClient.post<Viatura>('/viaturas', data).then(r => r.data);
export const updateViatura = (id: number, data: UpdateViaturaRequest) => apiClient.put<Viatura>(`/viaturas/${id}`, data).then(r => r.data);
export const deleteViatura = (id: number) => apiClient.delete(`/viaturas/${id}`);
