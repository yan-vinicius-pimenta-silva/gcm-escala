import { z } from 'zod';
export const posicaoSchema = z.object({
  nome: z.string().min(1, 'Nome é obrigatório'),
  ativo: z.boolean(),
});
export type PosicaoFormData = z.infer<typeof posicaoSchema>;
