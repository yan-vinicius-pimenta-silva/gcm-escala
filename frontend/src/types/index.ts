// Enums matching backend
export enum TipoSetor {
  Padrao = 'Padrao',
  CentralComunicacoes = 'CentralComunicacoes',
  RadioPatrulha = 'RadioPatrulha',
  DivisaoRural = 'DivisaoRural',
  Romu = 'Romu',
  RondaComercio = 'RondaComercio',
}

export enum MotivoAusencia {
  AtestadoMedico = 'AtestadoMedico',
  DoacaoSangue = 'DoacaoSangue',
  AfastamentoJudicial = 'AfastamentoJudicial',
  FaltaSemMotivo = 'FaltaSemMotivo',
  Outros = 'Outros',
}

export enum FuncaoAlocacao {
  Integrante = 'Integrante',
  Motorista = 'Motorista',
  Encarregado = 'Encarregado',
  Apoio = 'Apoio',
}

export enum StatusEscala {
  Rascunho = 'Rascunho',
  Publicada = 'Publicada',
  Fechada = 'Fechada',
}

export enum PerfilUsuario {
  Admin = 'Admin',
  Operador = 'Operador',
  Consulta = 'Consulta',
}

// Interfaces
export interface Setor {
  id: number;
  nome: string;
  tipo: TipoSetor;
  ativo: boolean;
}

export interface Posicao {
  id: number;
  nome: string;
  ativo: boolean;
}

export interface Turno {
  id: number;
  nome: string;
  ativo: boolean;
}

export interface Horario {
  id: number;
  inicio: string;
  fim: string;
  descricao: string;
  ativo: boolean;
}

export interface Viatura {
  id: number;
  identificador: string;
  ativo: boolean;
}

export interface Guarda {
  id: number;
  nome: string;
  telefone?: string;
  posicaoId: number;
  ativo: boolean;
  posicao?: Posicao;
}

export interface Equipe {
  id: number;
  nome: string;
  ativo: boolean;
  membros?: EquipeMembro[];
}

export interface EquipeMembro {
  id: number;
  equipeId: number;
  guardaId: number;
  guarda?: Guarda;
}

export interface Ferias {
  id: number;
  guardaId: number;
  dataInicio: string;
  dataFim: string;
  observacao?: string;
  guarda?: Guarda;
}

export interface Ausencia {
  id: number;
  guardaId: number;
  dataInicio: string;
  dataFim: string;
  motivo: MotivoAusencia;
  observacoes?: string;
  guarda?: Guarda;
}

export interface Escala {
  id: number;
  ano: number;
  mes: number;
  quinzena: number;
  setorId: number;
  setorNome?: string;
  status: StatusEscala;
  setor?: Setor;
  itens?: EscalaItem[];
}

export interface EscalaItem {
  id: number;
  escalaId: number;
  data: string;
  turnoId: number;
  turnoNome?: string;
  horarioId: number;
  horarioDescricao?: string;
  observacao?: string;
  turno?: Turno;
  horario?: Horario;
  alocacoes?: EscalaAlocacao[];
}

export interface EscalaAlocacao {
  id: number;
  escalaItemId: number;
  guardaId?: number;
  guardaNome?: string;
  equipeId?: number;
  equipeNome?: string;
  funcao: FuncaoAlocacao;
  viaturaId?: number;
  viaturaIdentificador?: string;
  guarda?: Guarda;
  equipe?: Equipe;
  viatura?: Viatura;
}

export interface LoginRequest {
  nomeUsuario: string;
  senha: string;
}

export interface LoginResponse {
  token: string;
  nomeCompleto: string;
  perfil: string;
}
