import { useEffect } from 'react';
import { Dialog, DialogTitle, DialogContent, DialogActions, Button, Autocomplete, TextField } from '@mui/material';
import { useForm, Controller } from 'react-hook-form';
import { zodResolver } from '@hookform/resolvers/zod';
import FormField from '../../components/forms/FormField';
import { feriasSchema, type FeriasFormData } from './feriasSchema';
import { useGuardas } from '../guardas/useGuardas';
import type { Ferias } from '../../types';

interface Props { open: boolean; onClose: () => void; onSubmit: (data: FeriasFormData) => void; editData?: Ferias | null; }

export default function FeriasForm({ open, onClose, onSubmit, editData }: Props) {
  const { data: guardas = [] } = useGuardas();
  const { control, handleSubmit, reset } = useForm<FeriasFormData>({
    resolver: zodResolver(feriasSchema),
    defaultValues: { guardaId: 0, dataInicio: '', dataFim: '', observacao: '' },
  });

  useEffect(() => {
    if (editData) reset({ guardaId: editData.guardaId, dataInicio: editData.dataInicio, dataFim: editData.dataFim, observacao: editData.observacao || '' });
    else reset({ guardaId: 0, dataInicio: '', dataFim: '', observacao: '' });
  }, [editData, reset, open]);

  return (
    <Dialog open={open} onClose={onClose} maxWidth="sm" fullWidth>
      <form onSubmit={handleSubmit(onSubmit)}>
        <DialogTitle>{editData ? 'Editar Férias' : 'Novas Férias'}</DialogTitle>
        <DialogContent>
          <Controller
            name="guardaId"
            control={control}
            render={({ field, fieldState: { error } }) => (
              <Autocomplete
                options={guardas.filter(g => g.ativo)}
                getOptionLabel={(g) => g.nome}
                value={guardas.find(g => g.id === field.value) || null}
                onChange={(_, val) => field.onChange(val?.id || 0)}
                renderInput={(params) => <TextField {...params} label="Guarda" margin="normal" error={!!error} helperText={error?.message} />}
              />
            )}
          />
          <FormField name="dataInicio" control={control} label="Data Início" type="date" />
          <FormField name="dataFim" control={control} label="Data Fim" type="date" />
          <FormField name="observacao" control={control} label="Observação" multiline rows={2} />
        </DialogContent>
        <DialogActions><Button onClick={onClose}>Cancelar</Button><Button type="submit" variant="contained">Salvar</Button></DialogActions>
      </form>
    </Dialog>
  );
}
