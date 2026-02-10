import apiClient from './client';
import type { Turno } from '../types';

export interface CreateTurnoRequest { nome: string; ativo: boolean; }
export interface UpdateTurnoRequest { nome: string; ativo: boolean; }

export const getTurnos = () => apiClient.get<Turno[]>('/turnos').then(r => r.data);
export const getTurno = (id: number) => apiClient.get<Turno>(`/turnos/${id}`).then(r => r.data);
export const createTurno = (data: CreateTurnoRequest) => apiClient.post<Turno>('/turnos', data).then(r => r.data);
export const updateTurno = (id: number, data: UpdateTurnoRequest) => apiClient.put<Turno>(`/turnos/${id}`, data).then(r => r.data);
export const deleteTurno = (id: number) => apiClient.delete(`/turnos/${id}`);
