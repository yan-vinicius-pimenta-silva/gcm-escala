import { useEffect } from 'react';
import { Dialog, DialogTitle, DialogContent, DialogActions, Button, Autocomplete, TextField, Typography, Chip } from '@mui/material';
import { useForm, Controller } from 'react-hook-form';
import { zodResolver } from '@hookform/resolvers/zod';
import FormField from '../../components/forms/FormField';
import SwitchField from '../../components/forms/SwitchField';
import { equipeSchema, type EquipeFormData } from './equipeSchema';
import { useGuardas } from '../guardas/useGuardas';
import type { Equipe } from '../../types';

interface Props { open: boolean; onClose: () => void; onSubmit: (data: EquipeFormData) => void; editData?: Equipe | null; }

export default function EquipeForm({ open, onClose, onSubmit, editData }: Props) {
  const { data: guardas = [] } = useGuardas();
  const { control, handleSubmit, reset, watch } = useForm<EquipeFormData>({
    resolver: zodResolver(equipeSchema),
    defaultValues: { nome: '', ativo: true, guardaIds: [] },
  });

  const selectedIds = watch('guardaIds');

  useEffect(() => {
    if (editData) {
      reset({ nome: editData.nome, ativo: editData.ativo, guardaIds: editData.membros?.map(m => m.guardaId) || [] });
    } else {
      reset({ nome: '', ativo: true, guardaIds: [] });
    }
  }, [editData, reset, open]);

  const activeGuardas = guardas.filter(g => g.ativo);

  return (
    <Dialog open={open} onClose={onClose} maxWidth="sm" fullWidth>
      <form onSubmit={handleSubmit(onSubmit)}>
        <DialogTitle>{editData ? 'Editar Equipe' : 'Nova Equipe'}</DialogTitle>
        <DialogContent>
          <FormField name="nome" control={control} label="Nome" />
          <Controller
            name="guardaIds"
            control={control}
            render={({ field, fieldState: { error } }) => (
              <>
                <Autocomplete
                  multiple
                  options={activeGuardas}
                  getOptionLabel={(g) => g.nome}
                  value={activeGuardas.filter(g => field.value.includes(g.id))}
                  onChange={(_, newVal) => field.onChange(newVal.map(g => g.id))}
                  renderTags={(value, getTagProps) => value.map((option, index) => (
                    <Chip label={option.nome} {...getTagProps({ index })} key={option.id} size="small" />
                  ))}
                  renderInput={(params) => (
                    <TextField {...params} label="Membros" margin="normal" error={!!error} helperText={error?.message} />
                  )}
                />
                <Typography variant="caption" color={selectedIds.length >= 2 && selectedIds.length <= 4 ? 'success.main' : 'error.main'}>
                  {selectedIds.length}/4 membros (mínimo 2)
                </Typography>
              </>
            )}
          />
          <SwitchField name="ativo" control={control} label="Ativo" />
        </DialogContent>
        <DialogActions><Button onClick={onClose}>Cancelar</Button><Button type="submit" variant="contained">Salvar</Button></DialogActions>
      </form>
    </Dialog>
  );
}
