import apiClient from './client';
import type { Horario } from '../types';

export interface CreateHorarioRequest { inicio: string; fim: string; descricao?: string; ativo: boolean; }
export interface UpdateHorarioRequest { inicio: string; fim: string; descricao?: string; ativo: boolean; }

export const getHorarios = () => apiClient.get<Horario[]>('/horarios').then(r => r.data);
export const getHorario = (id: number) => apiClient.get<Horario>(`/horarios/${id}`).then(r => r.data);
export const createHorario = (data: CreateHorarioRequest) => apiClient.post<Horario>('/horarios', data).then(r => r.data);
export const updateHorario = (id: number, data: UpdateHorarioRequest) => apiClient.put<Horario>(`/horarios/${id}`, data).then(r => r.data);
export const deleteHorario = (id: number) => apiClient.delete(`/horarios/${id}`);
