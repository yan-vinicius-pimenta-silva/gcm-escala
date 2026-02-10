import { z } from 'zod';

export const setorSchema = z.object({
  nome: z.string().min(1, 'Nome é obrigatório'),
  tipo: z.string().min(1, 'Tipo é obrigatório'),
  ativo: z.boolean(),
});

export type SetorFormData = z.infer<typeof setorSchema>;
