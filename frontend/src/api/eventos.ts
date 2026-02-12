import apiClient from './client';
import type { Evento } from '../types';
export interface CreateEventoRequest { nome: string; dataInicio: string; dataFim: string; }
export interface UpdateEventoRequest { nome: string; dataInicio: string; dataFim: string; }
export const getEventos = () => apiClient.get<Evento[]>('/eventos').then(r => r.data);
export const createEvento = (data: CreateEventoRequest) => apiClient.post<Evento>('/eventos', data).then(r => r.data);
export const updateEvento = (id: number, data: UpdateEventoRequest) => apiClient.put<Evento>(`/eventos/${id}`, data).then(r => r.data);
export const deleteEvento = (id: number) => apiClient.delete(`/eventos/${id}`);
