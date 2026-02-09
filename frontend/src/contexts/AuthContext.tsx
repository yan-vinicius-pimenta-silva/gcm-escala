import { createContext, useContext, useState, useCallback, type ReactNode } from 'react';
import apiClient from '../api/client';
import type { LoginRequest, LoginResponse } from '../types';

interface AuthContextType {
  token: string | null;
  nomeCompleto: string | null;
  perfil: string | null;
  isAuthenticated: boolean;
  login: (data: LoginRequest) => Promise<void>;
  logout: () => void;
}

const AuthContext = createContext<AuthContextType | null>(null);

export function AuthProvider({ children }: { children: ReactNode }) {
  const [token, setToken] = useState<string | null>(() => localStorage.getItem('token'));
  const [nomeCompleto, setNomeCompleto] = useState<string | null>(() => localStorage.getItem('nomeCompleto'));
  const [perfil, setPerfil] = useState<string | null>(() => localStorage.getItem('perfil'));

  const login = useCallback(async (data: LoginRequest) => {
    const response = await apiClient.post<LoginResponse>('/auth/login', data);
    const { token: t, nomeCompleto: n, perfil: p } = response.data;
    localStorage.setItem('token', t);
    localStorage.setItem('nomeCompleto', n);
    localStorage.setItem('perfil', p);
    setToken(t);
    setNomeCompleto(n);
    setPerfil(p);
  }, []);

  const logout = useCallback(() => {
    localStorage.removeItem('token');
    localStorage.removeItem('nomeCompleto');
    localStorage.removeItem('perfil');
    setToken(null);
    setNomeCompleto(null);
    setPerfil(null);
  }, []);

  return (
    <AuthContext.Provider value={{ token, nomeCompleto, perfil, isAuthenticated: !!token, login, logout }}>
      {children}
    </AuthContext.Provider>
  );
}

export function useAuth() {
  const context = useContext(AuthContext);
  if (!context) throw new Error('useAuth must be used within AuthProvider');
  return context;
}
