import { z } from 'zod';

export const horarioSchema = z.object({
  inicio: z.string().min(1, 'Hora início é obrigatória'),
  fim: z.string().min(1, 'Hora fim é obrigatória'),
  descricao: z.string().optional(),
  ativo: z.boolean(),
});

export type HorarioFormData = z.infer<typeof horarioSchema>;
