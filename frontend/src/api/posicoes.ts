import apiClient from './client';
import type { Posicao } from '../types';

export interface CreatePosicaoRequest { nome: string; ativo: boolean; }
export interface UpdatePosicaoRequest { nome: string; ativo: boolean; }

export const getPosicoes = () => apiClient.get<Posicao[]>('/posicoes').then(r => r.data);
export const getPosicao = (id: number) => apiClient.get<Posicao>(`/posicoes/${id}`).then(r => r.data);
export const createPosicao = (data: CreatePosicaoRequest) => apiClient.post<Posicao>('/posicoes', data).then(r => r.data);
export const updatePosicao = (id: number, data: UpdatePosicaoRequest) => apiClient.put<Posicao>(`/posicoes/${id}`, data).then(r => r.data);
export const deletePosicao = (id: number) => apiClient.delete(`/posicoes/${id}`);
