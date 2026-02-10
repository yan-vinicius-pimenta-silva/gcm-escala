import { useEffect } from 'react';
import { Dialog, DialogTitle, DialogContent, DialogActions, Button, Autocomplete, TextField } from '@mui/material';
import { useForm, Controller } from 'react-hook-form';
import { zodResolver } from '@hookform/resolvers/zod';
import FormField from '../../components/forms/FormField';
import SelectField from '../../components/forms/SelectField';
import { ausenciaSchema, type AusenciaFormData } from './ausenciaSchema';
import { useGuardas } from '../guardas/useGuardas';
import { MotivoAusencia } from '../../types';
import type { Ausencia } from '../../types';

const motivoOptions = [
  { value: MotivoAusencia.AtestadoMedico, label: 'Atestado Médico' },
  { value: MotivoAusencia.DoacaoSangue, label: 'Doação de Sangue' },
  { value: MotivoAusencia.AfastamentoJudicial, label: 'Afastamento Judicial' },
  { value: MotivoAusencia.FaltaSemMotivo, label: 'Falta sem Motivo' },
  { value: MotivoAusencia.Outros, label: 'Outros' },
];

interface Props { open: boolean; onClose: () => void; onSubmit: (data: AusenciaFormData) => void; editData?: Ausencia | null; }

export default function AusenciaForm({ open, onClose, onSubmit, editData }: Props) {
  const { data: guardas = [] } = useGuardas();
  const { control, handleSubmit, reset } = useForm<AusenciaFormData>({
    resolver: zodResolver(ausenciaSchema),
    defaultValues: { guardaId: 0, dataInicio: '', dataFim: '', motivo: '', observacoes: '' },
  });

  useEffect(() => {
    if (editData) reset({ guardaId: editData.guardaId, dataInicio: editData.dataInicio, dataFim: editData.dataFim, motivo: editData.motivo, observacoes: editData.observacoes || '' });
    else reset({ guardaId: 0, dataInicio: '', dataFim: '', motivo: '', observacoes: '' });
  }, [editData, reset, open]);

  return (
    <Dialog open={open} onClose={onClose} maxWidth="sm" fullWidth>
      <form onSubmit={handleSubmit(onSubmit)}>
        <DialogTitle>{editData ? 'Editar Ausência' : 'Nova Ausência'}</DialogTitle>
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
          <SelectField name="motivo" control={control} label="Motivo" options={motivoOptions} />
          <FormField name="observacoes" control={control} label="Observações" multiline rows={2} />
        </DialogContent>
        <DialogActions><Button onClick={onClose}>Cancelar</Button><Button type="submit" variant="contained">Salvar</Button></DialogActions>
      </form>
    </Dialog>
  );
}
