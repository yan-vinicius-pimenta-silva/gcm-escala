import apiClient from './client';
import type { Equipe } from '../types';
export interface CreateEquipeRequest { nome: string; ativo: boolean; guardaIds: number[]; }
export interface UpdateEquipeRequest { nome: string; ativo: boolean; guardaIds: number[]; }
export const getEquipes = () => apiClient.get<Equipe[]>('/equipes').then(r => r.data);
export const createEquipe = (data: CreateEquipeRequest) => apiClient.post<Equipe>('/equipes', data).then(r => r.data);
export const updateEquipe = (id: number, data: UpdateEquipeRequest) => apiClient.put<Equipe>(`/equipes/${id}`, data).then(r => r.data);
export const deleteEquipe = (id: number) => apiClient.delete(`/equipes/${id}`);
