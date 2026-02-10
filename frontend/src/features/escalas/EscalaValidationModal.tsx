import { Dialog, DialogTitle, DialogContent, DialogActions, Button, List, ListItem, ListItemIcon, ListItemText } from '@mui/material';
import BeachAccessIcon from '@mui/icons-material/BeachAccess';
import EventBusyIcon from '@mui/icons-material/EventBusy';
import WarningIcon from '@mui/icons-material/Warning';
import RuleIcon from '@mui/icons-material/Rule';
import type { ConflictError } from '../../api/escalas';

const iconMap: Record<string, React.ReactNode> = {
  FERIAS: <BeachAccessIcon color="warning" />,
  AUSENCIA: <EventBusyIcon color="error" />,
  CONFLITO_ESCALA: <WarningIcon color="error" />,
  REGRA_SETOR: <RuleIcon color="info" />,
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
