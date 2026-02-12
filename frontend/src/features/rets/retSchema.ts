import { z } from 'zod';
export const retSchema = z.object({
  guardaId: z.number({ error: 'Guarda é obrigatório' }).min(1, 'Guarda é obrigatório'),
  data: z.string().min(1, 'Data é obrigatória'),
  horarioInicio: z.string().min(1, 'Horário de início é obrigatório'),
  tipo: z.string().min(1, 'Tipo é obrigatório'),
  eventoId: z.number().optional(),
  observacao: z.string().optional(),
});
export type RetFormData = z.infer<typeof retSchema>;
