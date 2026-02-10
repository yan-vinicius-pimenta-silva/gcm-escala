import { useEffect } from 'react';
import { Dialog, DialogTitle, DialogContent, DialogActions, Button } from '@mui/material';
import { useForm } from 'react-hook-form';
import { zodResolver } from '@hookform/resolvers/zod';
import FormField from '../../components/forms/FormField';
import SwitchField from '../../components/forms/SwitchField';
import { horarioSchema, type HorarioFormData } from './horarioSchema';
import type { Horario } from '../../types';

interface Props {
  open: boolean;
  onClose: () => void;
  onSubmit: (data: HorarioFormData) => void;
  editData?: Horario | null;
}

export default function HorarioForm({ open, onClose, onSubmit, editData }: Props) {
  const { control, handleSubmit, reset } = useForm<HorarioFormData>({
    resolver: zodResolver(horarioSchema),
    defaultValues: { inicio: '', fim: '', descricao: '', ativo: true },
  });

  useEffect(() => {
    if (editData) reset({ inicio: editData.inicio, fim: editData.fim, descricao: editData.descricao, ativo: editData.ativo });
    else reset({ inicio: '', fim: '', descricao: '', ativo: true });
  }, [editData, reset, open]);

  return (
    <Dialog open={open} onClose={onClose} maxWidth="sm" fullWidth>
      <form onSubmit={handleSubmit(onSubmit)}>
        <DialogTitle>{editData ? 'Editar Horário' : 'Novo Horário'}</DialogTitle>
        <DialogContent>
          <FormField name="inicio" control={control} label="Hora Início (HH:mm)" />
          <FormField name="fim" control={control} label="Hora Fim (HH:mm)" />
          <FormField name="descricao" control={control} label="Descrição (opcional)" />
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
