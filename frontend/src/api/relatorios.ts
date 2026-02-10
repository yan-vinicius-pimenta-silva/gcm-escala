import apiClient from './client';

export type TipoRelatorio =
  | 'EscalaMensalPorSetor'
  | 'Setores'
  | 'Posicoes'
  | 'Turnos'
  | 'Horarios'
  | 'Equipes'
  | 'Viaturas'
  | 'Guardas'
  | 'Escalas'
  | 'GuardasEscalados'
  | 'GuardasNaoEscalados'
  | 'Ferias'
  | 'Ausencias'
  | 'IndividualGuarda';

export interface RelatorioRequest {
  tipo: TipoRelatorio;
  mes: number;
  ano: number;
  setorId?: number;
  guardaId?: number;
  mesFim?: number;
  anoFim?: number;
}

export interface RelatorioResult {
  tituloRelatorio: string;
  colunas: string[];
  linhas: Record<string, string>[];
}

export const gerarRelatorio = (data: RelatorioRequest) =>
  apiClient.post<RelatorioResult>('/relatorios/gerar', data).then(r => r.data);

export const exportarExcel = (data: RelatorioRequest) =>
  apiClient.post('/relatorios/excel', data, { responseType: 'blob' }).then(r => {
    const url = window.URL.createObjectURL(new Blob([r.data]));
    const link = document.createElement('a');
    link.href = url;
    link.setAttribute('download', `relatorio_${data.tipo}_${data.mes}_${data.ano}.xlsx`);
    document.body.appendChild(link);
    link.click();
    link.remove();
    window.URL.revokeObjectURL(url);
  });


export const exportarPdf = (data: RelatorioRequest) =>
  apiClient.post('/relatorios/pdf', data, { responseType: 'blob' }).then(r => {
    const url = window.URL.createObjectURL(new Blob([r.data], { type: 'application/pdf' }));
    const link = document.createElement('a');
    link.href = url;
    link.setAttribute('download', `relatorio_${data.tipo}_${data.mes}_${data.ano}.pdf`);
    document.body.appendChild(link);
    link.click();
    link.remove();
    window.URL.revokeObjectURL(url);
  });
