import { useEffect } from 'react';
import { Dialog, DialogTitle, DialogContent, DialogActions, Button } from '@mui/material';
import { useForm } from 'react-hook-form';
import { zodResolver } from '@hookform/resolvers/zod';
import FormField from '../../components/forms/FormField';
import SelectField from '../../components/forms/SelectField';
import SwitchField from '../../components/forms/SwitchField';
import { guardaSchema, type GuardaFormData } from './guardaSchema';
import { usePosicoes } from '../posicoes/usePosicoes';
import type { Guarda } from '../../types';

interface Props {
  open: boolean;
  onClose: () => void;
  onSubmit: (data: GuardaFormData) => void;
  editData?: Guarda | null;
}

export default function GuardaForm({ open, onClose, onSubmit, editData }: Props) {
  const { data: posicoes = [] } = usePosicoes();
  const { control, handleSubmit, reset } = useForm<GuardaFormData>({
    resolver: zodResolver(guardaSchema),
    defaultValues: { nome: '', telefone: '', posicaoId: 0, ativo: true },
  });

  useEffect(() => {
    if (editData) reset({ nome: editData.nome, telefone: editData.telefone || '', posicaoId: editData.posicaoId, ativo: editData.ativo });
    else reset({ nome: '', telefone: '', posicaoId: 0, ativo: true });
  }, [editData, reset, open]);

  const posicaoOptions = posicoes.filter(p => p.ativo).map(p => ({ value: p.id, label: p.nome }));

  return (
    <Dialog open={open} onClose={onClose} maxWidth="sm" fullWidth>
      <form onSubmit={handleSubmit(onSubmit)}>
        <DialogTitle>{editData ? 'Editar Guarda' : 'Novo Guarda'}</DialogTitle>
        <DialogContent>
          <FormField name="nome" control={control} label="Nome" />
          <FormField name="telefone" control={control} label="Telefone" />
          <SelectField name="posicaoId" control={control} label="Posição" options={posicaoOptions} />
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
