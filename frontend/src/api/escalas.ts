import apiClient from './client';
import type { Escala } from '../types';

export interface CreateEscalaRequest { ano: number; mes: number; quinzena: number; setorId: number; }
export interface AlocacaoRequest { guardaId?: number; equipeId?: number; funcao: string; viaturaId?: number; }
export interface AddEscalaItemRequest { data: string; turnoId: number; horarioId: number; observacao?: string; alocacoes: AlocacaoRequest[]; }
export interface UpdateEscalaItemRequest { data: string; turnoId: number; horarioId: number; observacao?: string; alocacoes: AlocacaoRequest[]; }
export interface ConflictError { tipo: string; mensagem: string; }
export interface EscalaFiltersParams { ano?: number; mes?: number; quinzena?: number; setorId?: number; }

export const getEscalas = (params: EscalaFiltersParams) => apiClient.get<Escala[]>('/escalas', { params }).then(r => r.data);
export const getEscala = (id: number) => apiClient.get<Escala>(`/escalas/${id}`).then(r => r.data);
export const createEscala = (data: CreateEscalaRequest) => apiClient.post<Escala>('/escalas', data).then(r => r.data);
export const addEscalaItem = (escalaId: number, data: AddEscalaItemRequest) => apiClient.post(`/escalas/${escalaId}/itens`, data).then(r => r.data);
export const updateEscalaItem = (escalaId: number, itemId: number, data: UpdateEscalaItemRequest) => apiClient.put(`/escalas/${escalaId}/itens/${itemId}`, data).then(r => r.data);
export const deleteEscalaItem = (escalaId: number, itemId: number) => apiClient.delete(`/escalas/${escalaId}/itens/${itemId}`);
export const publicarEscala = (id: number) => apiClient.post(`/escalas/${id}/publicar`).then(r => r.data);
export const deleteEscala = (id: number) => apiClient.delete(`/escalas/${id}`);
