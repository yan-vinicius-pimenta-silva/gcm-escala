import { z } from 'zod';
export const equipeSchema = z.object({
  nome: z.string().min(1, 'Nome é obrigatório'),
  ativo: z.boolean(),
  guardaIds: z.array(z.number()).min(2, 'Mínimo 2 membros').max(4, 'Máximo 4 membros'),
});
export type EquipeFormData = z.infer<typeof equipeSchema>;
