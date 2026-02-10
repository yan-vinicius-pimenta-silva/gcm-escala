import { useEffect } from 'react';
import { Dialog, DialogTitle, DialogContent, DialogActions, Button } from '@mui/material';
import { useForm } from 'react-hook-form';
import { zodResolver } from '@hookform/resolvers/zod';
import FormField from '../../components/forms/FormField';
import SwitchField from '../../components/forms/SwitchField';
import { turnoSchema, type TurnoFormData } from './turnoSchema';
import type { Turno } from '../../types';

interface Props {
  open: boolean;
  onClose: () => void;
  onSubmit: (data: TurnoFormData) => void;
  editData?: Turno | null;
}

export default function TurnoForm({ open, onClose, onSubmit, editData }: Props) {
  const { control, handleSubmit, reset } = useForm<TurnoFormData>({
    resolver: zodResolver(turnoSchema),
    defaultValues: { nome: '', ativo: true },
  });

  useEffect(() => {
    if (editData) reset({ nome: editData.nome, ativo: editData.ativo });
    else reset({ nome: '', ativo: true });
  }, [editData, reset, open]);

  return (
    <Dialog open={open} onClose={onClose} maxWidth="sm" fullWidth>
      <form onSubmit={handleSubmit(onSubmit)}>
        <DialogTitle>{editData ? 'Editar Turno' : 'Novo Turno'}</DialogTitle>
        <DialogContent>
          <FormField name="nome" control={control} label="Nome" />
          <SwitchField name="ativo" control={control} label="Ativo" />
        </DialogContent>
        <DialogActions>
          <Button onClick={onClose}>Cancelar</Button>
          <Button type="submit" variant="contained">Salvar</Button>
        </DialogActions>
      </form>
    </Dialog>
  );
}
