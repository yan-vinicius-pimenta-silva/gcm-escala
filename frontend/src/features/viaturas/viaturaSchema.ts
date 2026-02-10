import { z } from 'zod';
export const viaturaSchema = z.object({ identificador: z.string().min(1, 'Identificador é obrigatório'), ativo: z.boolean() });
export type ViaturaFormData = z.infer<typeof viaturaSchema>;
