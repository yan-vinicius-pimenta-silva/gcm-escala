import { useMutation } from '@tanstack/react-query';
import { gerarRelatorio, exportarExcel } from '../../api/relatorios';
import type { RelatorioRequest } from '../../api/relatorios';

export function useGerarRelatorio() {
  return useMutation({ mutationFn: (data: RelatorioRequest) => gerarRelatorio(data) });
}

export function useExportarExcel() {
  return useMutation({ mutationFn: (data: RelatorioRequest) => exportarExcel(data) });
}
