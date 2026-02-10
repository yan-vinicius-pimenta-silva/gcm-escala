import { z } from 'zod';
export const turnoSchema = z.object({
  nome: z.string().min(1, 'Nome é obrigatório'),
  ativo: z.boolean(),
});
export type TurnoFormData = z.infer<typeof turnoSchema>;
