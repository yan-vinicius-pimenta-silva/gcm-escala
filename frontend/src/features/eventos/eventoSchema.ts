import { z } from 'zod';
export const eventoSchema = z.object({
  nome: z.string().min(1, 'Nome é obrigatório'),
  dataInicio: z.string().min(1, 'Data início é obrigatória'),
  dataFim: z.string().min(1, 'Data fim é obrigatória'),
});
export type EventoFormData = z.infer<typeof eventoSchema>;
