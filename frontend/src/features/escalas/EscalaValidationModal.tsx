import { Dialog, DialogTitle, DialogContent, DialogActions, Button, List, ListItem, ListItemIcon, ListItemText } from '@mui/material';
import BeachAccessIcon from '@mui/icons-material/BeachAccess';
import EventBusyIcon from '@mui/icons-material/EventBusy';
import WarningIcon from '@mui/icons-material/Warning';
import RuleIcon from '@mui/icons-material/Rule';
import PeopleIcon from '@mui/icons-material/People';
import HourglassBottomIcon from '@mui/icons-material/HourglassBottom';
import SecurityIcon from '@mui/icons-material/Security';
import type { ConflictError } from '../../api/escalas';

const iconMap: Record<string, React.ReactNode> = {
  FERIAS: <BeachAccessIcon color="warning" />,
  AUSENCIA: <EventBusyIcon color="error" />,
  CONFLITO_ESCALA: <WarningIcon color="error" />,
  REGRA_SETOR: <RuleIcon color="info" />,
  GUARDA_DUPLICADO: <PeopleIcon color="error" />,
  FOLGA_12X36: <HourglassBottomIcon color="warning" />,
  RET: <SecurityIcon color="info" />,
};

interface Props {
  open: boolean;
  errors: ConflictError[];
  onClose: () => void;
}

export default function EscalaValidationModal({ open, errors, onClose }: Props) {
  return (
    <Dialog open={open} onClose={onClose} maxWidth="sm" fullWidth>
      <DialogTitle>Erros de Validacao</DialogTitle>
      <DialogContent>
        <List>
          {errors.map((e, i) => (
            <ListItem key={i}>
              <ListItemIcon>{iconMap[e.tipo] || <WarningIcon />}</ListItemIcon>
              <ListItemText primary={e.mensagem} />
            </ListItem>
          ))}
        </List>
      </DialogContent>
      <DialogActions>
        <Button onClick={onClose} variant="contained">Fechar</Button>
      </DialogActions>
    </Dialog>
  );
}
