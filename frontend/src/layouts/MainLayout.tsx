import { useState } from 'react';
import { Outlet, useNavigate, useLocation } from 'react-router-dom';
import {
  AppBar, Box, Drawer, IconButton, List, ListItemButton, ListItemIcon, ListItemText,
  Toolbar, Typography, Divider, Button,
} from '@mui/material';
import MenuIcon from '@mui/icons-material/Menu';
import DomainIcon from '@mui/icons-material/Domain';
import BadgeIcon from '@mui/icons-material/Badge';
import ScheduleIcon from '@mui/icons-material/Schedule';
import AccessTimeIcon from '@mui/icons-material/AccessTime';
import GroupsIcon from '@mui/icons-material/Groups';
import DirectionsCarIcon from '@mui/icons-material/DirectionsCar';
import PersonIcon from '@mui/icons-material/Person';
import CalendarMonthIcon from '@mui/icons-material/CalendarMonth';
import BeachAccessIcon from '@mui/icons-material/BeachAccess';
import EventBusyIcon from '@mui/icons-material/EventBusy';
import AssessmentIcon from '@mui/icons-material/Assessment';
import { useAuth } from '../contexts/AuthContext';

const DRAWER_WIDTH = 260;

const menuItems = [
  { label: 'Setores', path: '/setores', icon: <DomainIcon /> },
  { label: 'Posições', path: '/posicoes', icon: <BadgeIcon /> },
  { label: 'Turnos', path: '/turnos', icon: <ScheduleIcon /> },
  { label: 'Horários', path: '/horarios', icon: <AccessTimeIcon /> },
  { label: 'Equipes', path: '/equipes', icon: <GroupsIcon /> },
  { label: 'Viaturas', path: '/viaturas', icon: <DirectionsCarIcon /> },
  { label: 'Guardas', path: '/guardas', icon: <PersonIcon /> },
  { label: 'Escalas', path: '/escalas', icon: <CalendarMonthIcon /> },
  { label: 'Férias', path: '/ferias', icon: <BeachAccessIcon /> },
  { label: 'Ausências', path: '/ausencias', icon: <EventBusyIcon /> },
  { label: 'Relatórios', path: '/relatorios', icon: <AssessmentIcon /> },
];

export default function MainLayout() {
  const [drawerOpen, setDrawerOpen] = useState(true);
  const navigate = useNavigate();
  const location = useLocation();
  const { nomeCompleto, logout } = useAuth();

  return (
    <Box sx={{ display: 'flex' }}>
      <AppBar position="fixed" sx={{ zIndex: (theme) => theme.zIndex.drawer + 1 }}>
        <Toolbar>
          <IconButton color="inherit" edge="start" onClick={() => setDrawerOpen(!drawerOpen)} sx={{ mr: 2 }}>
            <MenuIcon />
          </IconButton>
          <Typography variant="h6" noWrap sx={{ flexGrow: 1 }}>
            Escalas GCM
          </Typography>
          <Typography variant="body2" sx={{ mr: 2 }}>{nomeCompleto}</Typography>
          <Button color="inherit" onClick={() => { logout(); navigate('/login'); }}>Sair</Button>
        </Toolbar>
      </AppBar>
      <Drawer
        variant="persistent" open={drawerOpen}
        sx={{
          width: DRAWER_WIDTH,
          flexShrink: 0,
          '& .MuiDrawer-paper': { width: DRAWER_WIDTH, boxSizing: 'border-box' },
        }}
      >
        <Toolbar />
        <Divider />
        <List>
          {menuItems.map((item) => (
            <ListItemButton
              key={item.path}
              selected={location.pathname === item.path}
              onClick={() => navigate(item.path)}
            >
              <ListItemIcon>{item.icon}</ListItemIcon>
              <ListItemText primary={item.label} />
            </ListItemButton>
          ))}
        </List>
      </Drawer>
      <Box component="main" sx={{ flexGrow: 1, p: 3, ml: drawerOpen ? `${DRAWER_WIDTH}px` : 0, transition: 'margin 0.3s' }}>
        <Toolbar />
        <Outlet />
      </Box>
    </Box>
  );
}
