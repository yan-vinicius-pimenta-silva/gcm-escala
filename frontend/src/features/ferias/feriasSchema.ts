import { z } from 'zod';
export const feriasSchema = z.object({
  guardaId: z.number({ required_error: 'Guarda é obrigatório' }).min(1, 'Guarda é obrigatório'),
  dataInicio: z.string().min(1, 'Data início é obrigatória'),
  dataFim: z.string().min(1, 'Data fim é obrigatória'),
  observacao: z.string().optional(),
});
export type FeriasFormData = z.infer<typeof feriasSchema>;
