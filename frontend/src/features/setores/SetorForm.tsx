import { useEffect } from 'react';
import { Dialog, DialogTitle, DialogContent, DialogActions, Button } from '@mui/material';
import { useForm } from 'react-hook-form';
import { zodResolver } from '@hookform/resolvers/zod';
import FormField from '../../components/forms/FormField';
import SelectField from '../../components/forms/SelectField';
import SwitchField from '../../components/forms/SwitchField';
import { setorSchema, type SetorFormData } from './setorSchema';
import type { Setor } from '../../types';
import { TipoSetor } from '../../types';

const tipoOptions = Object.values(TipoSetor).map(t => ({ value: t, label: t }));

interface SetorFormProps {
  open: boolean;
  onClose: () => void;
  onSubmit: (data: SetorFormData) => void;
  editData?: Setor | null;
}

export default function SetorForm({ open, onClose, onSubmit, editData }: SetorFormProps) {
  const { control, handleSubmit, reset } = useForm<SetorFormData>({
    resolver: zodResolver(setorSchema),
    defaultValues: { nome: '', tipo: 'Padrao', ativo: true },
  });

  useEffect(() => {
    if (editData) {
      reset({ nome: editData.nome, tipo: editData.tipo, ativo: editData.ativo });
    } else {
      reset({ nome: '', tipo: 'Padrao', ativo: true });
    }
  }, [editData, reset, open]);

  return (
    <Dialog open={open} onClose={onClose} maxWidth="sm" fullWidth>
      <form onSubmit={handleSubmit(onSubmit)}>
        <DialogTitle>{editData ? 'Editar Setor' : 'Novo Setor'}</DialogTitle>
        <DialogContent>
          <FormField name="nome" control={control} label="Nome" />
          <SelectField name="tipo" control={control} label="Tipo" options={tipoOptions} />
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
