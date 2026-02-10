import { z } from 'zod';

export const guardaSchema = z.object({
  nome: z.string().min(1, 'Nome é obrigatório'),
  telefone: z.string().optional(),
  posicaoId: z.number({ required_error: 'Posição é obrigatória' }).min(1, 'Posição é obrigatória'),
  ativo: z.boolean(),
});

export type GuardaFormData = z.infer<typeof guardaSchema>;
