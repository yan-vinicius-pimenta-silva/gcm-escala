import { z } from 'zod';
export const ausenciaSchema = z.object({
  guardaId: z.number({ required_error: 'Guarda é obrigatório' }).min(1, 'Guarda é obrigatório'),
  dataInicio: z.string().min(1, 'Data início é obrigatória'),
  dataFim: z.string().min(1, 'Data fim é obrigatória'),
  motivo: z.string().min(1, 'Motivo é obrigatório'),
  observacoes: z.string().optional(),
});
export type AusenciaFormData = z.infer<typeof ausenciaSchema>;
