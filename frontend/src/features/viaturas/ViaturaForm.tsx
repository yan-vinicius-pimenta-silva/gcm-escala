import { useEffect } from 'react';
import { Dialog, DialogTitle, DialogContent, DialogActions, Button } from '@mui/material';
import { useForm } from 'react-hook-form';
import { zodResolver } from '@hookform/resolvers/zod';
import FormField from '../../components/forms/FormField';
import SwitchField from '../../components/forms/SwitchField';
import { viaturaSchema, type ViaturaFormData } from './viaturaSchema';
import type { Viatura } from '../../types';

interface Props { open: boolean; onClose: () => void; onSubmit: (data: ViaturaFormData) => void; editData?: Viatura | null; }

export default function ViaturaForm({ open, onClose, onSubmit, editData }: Props) {
  const { control, handleSubmit, reset } = useForm<ViaturaFormData>({ resolver: zodResolver(viaturaSchema), defaultValues: { identificador: '', ativo: true } });
  useEffect(() => { if (editData) reset({ identificador: editData.identificador, ativo: editData.ativo }); else reset({ identificador: '', ativo: true }); }, [editData, reset, open]);
  return (
    <Dialog open={open} onClose={onClose} maxWidth="sm" fullWidth>
      <form onSubmit={handleSubmit(onSubmit)}>
        <DialogTitle>{editData ? 'Editar Viatura' : 'Nova Viatura'}</DialogTitle>
        <DialogContent>
          <FormField name="identificador" control={control} label="Identificador (ex: VTR 13400)" maxLength={100} />
          <SwitchField name="ativo" control={control} label="Ativo" />
        </DialogContent>
        <DialogActions><Button onClick={onClose}>Cancelar</Button><Button type="submit" variant="contained">Salvar</Button></DialogActions>
      </form>
    </Dialog>
  );
}
