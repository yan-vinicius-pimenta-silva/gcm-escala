import { useEffect } from 'react';
import { Dialog, DialogTitle, DialogContent, DialogActions, Button } from '@mui/material';
import { useForm } from 'react-hook-form';
import { zodResolver } from '@hookform/resolvers/zod';
import FormField from '../../components/forms/FormField';
import { eventoSchema, type EventoFormData } from './eventoSchema';
import type { Evento } from '../../types';

interface Props { open: boolean; onClose: () => void; onSubmit: (data: EventoFormData) => void; editData?: Evento | null; }

export default function EventoForm({ open, onClose, onSubmit, editData }: Props) {
  const { control, handleSubmit, reset } = useForm<EventoFormData>({
    resolver: zodResolver(eventoSchema),
    defaultValues: { nome: '', dataInicio: '', dataFim: '' },
  });

  useEffect(() => {
    if (editData) reset({ nome: editData.nome, dataInicio: editData.dataInicio, dataFim: editData.dataFim });
    else reset({ nome: '', dataInicio: '', dataFim: '' });
  }, [editData, reset, open]);

  return (
    <Dialog open={open} onClose={onClose} maxWidth="sm" fullWidth>
      <form onSubmit={handleSubmit(onSubmit)}>
        <DialogTitle>{editData ? 'Editar Evento' : 'Novo Evento'}</DialogTitle>
        <DialogContent>
          <FormField name="nome" control={control} label="Nome" />
          <FormField name="dataInicio" control={control} label="Data Início" type="date" />
          <FormField name="dataFim" control={control} label="Data Fim" type="date" />
        </DialogContent>
        <DialogActions><Button onClick={onClose}>Cancelar</Button><Button type="submit" variant="contained">Salvar</Button></DialogActions>
      </form>
    </Dialog>
  );
}
