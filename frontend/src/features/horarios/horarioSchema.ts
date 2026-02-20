import { z } from 'zod';

const timeString = (label: string) =>
  z.string().min(1, `${label} é obrigatória`).regex(/^\d{2}:\d{2}$/, `${label} inválida (use HH:MM)`);

export const horarioSchema = z.object({
  inicio: timeString('Hora início'),
  fim: timeString('Hora fim'),
  descricao: z.string().optional(),
  ativo: z.boolean(),
});

export type HorarioFormData = z.infer<typeof horarioSchema>;
