import { useEffect } from 'react';
import { Dialog, DialogTitle, DialogContent, DialogActions, Button, Autocomplete, TextField } from '@mui/material';
import { useForm, Controller, useWatch } from 'react-hook-form';
import { zodResolver } from '@hookform/resolvers/zod';
import FormField from '../../components/forms/FormField';
import SelectField from '../../components/forms/SelectField';
import { retSchema, type RetFormData } from './retSchema';
import { useGuardas } from '../guardas/useGuardas';
import { useEventos } from '../eventos/useEventos';
import { TipoRet } from '../../types';
import type { Ret } from '../../types';

const tipoOptions = [
  { value: TipoRet.Mensal, label: 'Mensal' },
  { value: TipoRet.Evento, label: 'Evento' },
];

function addHours(time: string, hours: number): string {
  if (!time) return '';
  const [h, m] = time.split(':').map(Number);
  const totalMinutes = (h + hours) * 60 + m;
  const newH = Math.floor(totalMinutes / 60) % 24;
  const newM = totalMinutes % 60;
  return `${String(newH).padStart(2, '0')}:${String(newM).padStart(2, '0')}`;
}

interface Props { open: boolean; onClose: () => void; onSubmit: (data: RetFormData) => void; editData?: Ret | null; }

export default function RetForm({ open, onClose, onSubmit, editData }: Props) {
  const { data: guardas = [] } = useGuardas();
  const { data: eventos = [] } = useEventos();
  const { control, handleSubmit, reset, setValue } = useForm<RetFormData>({
    resolver: zodResolver(retSchema),
    defaultValues: { guardaId: 0, data: '', horarioInicio: '', tipo: '', eventoId: undefined, observacao: '' },
  });

  const tipo = useWatch({ control, name: 'tipo' });
  const horarioInicio = useWatch({ control, name: 'horarioInicio' });

  useEffect(() => {
    if (tipo !== TipoRet.Evento) {
      setValue('eventoId', undefined);
    }
  }, [tipo, setValue]);

  useEffect(() => {
    if (editData) reset({
      guardaId: editData.guardaId,
      data: editData.data,
      horarioInicio: editData.horarioInicio,
      tipo: editData.tipo,
      eventoId: editData.eventoId ?? undefined,
      observacao: editData.observacao || '',
    });
    else reset({ guardaId: 0, data: '', horarioInicio: '', tipo: '', eventoId: undefined, observacao: '' });
  }, [editData, reset, open]);

  return (
    <Dialog open={open} onClose={onClose} maxWidth="sm" fullWidth>
      <form onSubmit={handleSubmit(onSubmit)}>
        <DialogTitle>{editData ? 'Editar RET' : 'Novo RET'}</DialogTitle>
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
          <FormField name="data" control={control} label="Data" type="date" />
          <FormField name="horarioInicio" control={control} label="Horário Início" type="time" />
          <TextField
            label="Horário Fim (calculado)"
            value={addHours(horarioInicio, 8)}
            fullWidth
            margin="normal"
            disabled
            InputLabelProps={{ shrink: true }}
          />
          <SelectField name="tipo" control={control} label="Tipo" options={tipoOptions} />
          {tipo === TipoRet.Evento && (
            <Controller
              name="eventoId"
              control={control}
              render={({ field, fieldState: { error } }) => (
                <Autocomplete
                  options={eventos}
                  getOptionLabel={(e) => e.nome}
                  value={eventos.find(e => e.id === field.value) || null}
                  onChange={(_, val) => field.onChange(val?.id || undefined)}
                  renderInput={(params) => <TextField {...params} label="Evento" margin="normal" error={!!error} helperText={error?.message} />}
                />
              )}
            />
          )}
          <FormField name="observacao" control={control} label="Observação" multiline rows={2} />
        </DialogContent>
        <DialogActions><Button onClick={onClose}>Cancelar</Button><Button type="submit" variant="contained">Salvar</Button></DialogActions>
      </form>
    </Dialog>
  );
}
