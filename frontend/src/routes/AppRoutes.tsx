import { BrowserRouter, Routes, Route, Navigate } from 'react-router-dom';
import MainLayout from '../layouts/MainLayout';
import LoginPage from '../features/auth/LoginPage';
import { useAuth } from '../contexts/AuthContext';
import SetoresPage from '../features/setores/SetoresPage';
import PosicoesPage from '../features/posicoes/PosicoesPage';
import TurnosPage from '../features/turnos/TurnosPage';
import HorariosPage from '../features/horarios/HorariosPage';
import GuardasPage from '../features/guardas/GuardasPage';
import ViaturasPage from '../features/viaturas/ViaturasPage';
import EquipesPage from '../features/equipes/EquipesPage';
import FeriasPage from '../features/ferias/FeriasPage';
import AusenciasPage from '../features/ausencias/AusenciasPage';
import EscalasPage from '../features/escalas/EscalasPage';
import RelatoriosPage from '../features/relatorios/RelatoriosPage';

function PrivateRoute({ children }: { children: React.ReactNode }) {
  const { isAuthenticated } = useAuth();
  return isAuthenticated ? <>{children}</> : <Navigate to="/login" />;
}

export default function AppRoutes() {
  return (
    <BrowserRouter>
      <Routes>
        <Route path="/login" element={<LoginPage />} />
        <Route path="/" element={<PrivateRoute><MainLayout /></PrivateRoute>}>
          <Route index element={<Navigate to="/setores" />} />
          <Route path="setores" element={<SetoresPage />} />
          <Route path="posicoes" element={<PosicoesPage />} />
          <Route path="turnos" element={<TurnosPage />} />
          <Route path="horarios" element={<HorariosPage />} />
          <Route path="guardas" element={<GuardasPage />} />
          <Route path="viaturas" element={<ViaturasPage />} />
          <Route path="equipes" element={<EquipesPage />} />
          <Route path="ferias" element={<FeriasPage />} />
          <Route path="ausencias" element={<AusenciasPage />} />
          <Route path="escalas" element={<EscalasPage />} />
          <Route path="relatorios" element={<RelatoriosPage />} />
        </Route>
      </Routes>
    </BrowserRouter>
  );
}
