import { useState } from 'react';
import { Box, Typography } from '@mui/material';
import { DataGrid, type GridColDef } from '@mui/x-data-grid';
import { useSnackbar } from 'notistack';
import PageHeader from '../../components/ui/PageHeader';
import RelatorioFilters from './RelatorioFilters';
import { useGerarRelatorio, useExportarExcel, useExportarPdf } from './useRelatorios';
import type { TipoRelatorio, RelatorioResult } from '../../api/relatorios';
import { formatDateToDisplay, isIsoDateString } from '../../utils/date';

export default function RelatoriosPage() {
  const { enqueueSnackbar } = useSnackbar();
  const [tipo, setTipo] = useState<TipoRelatorio>('EscalaMensalPorSetor');
  const [mes, setMes] = useState(new Date().getMonth() + 1);
  const [ano, setAno] = useState(new Date().getFullYear());
  const [mesFim, setMesFim] = useState(new Date().getMonth() + 1);
  const [anoFim, setAnoFim] = useState(new Date().getFullYear());
  const [setorId, setSetorId] = useState<number | null>(null);
  const [guardaId, setGuardaId] = useState<number | null>(null);
  const [resultado, setResultado] = useState<RelatorioResult | null>(null);

  const gerarMut = useGerarRelatorio();
  const exportarExcelMut = useExportarExcel();
  const exportarPdfMut = useExportarPdf();

  const buildRequest = () => ({
    tipo,
    mes,
    ano,
    ...(tipo === 'Escalas' ? { mesFim, anoFim } : {}),
    ...(setorId ? { setorId } : {}),
    ...(guardaId ? { guardaId } : {}),
  });

  async function handleGerar() {
    try {
      const result = await gerarMut.mutateAsync(buildRequest());
      setResultado(result);
    } catch (err: unknown) {
      const message = (err as { response?: { data?: { message?: string } } })?.response?.data?.message;
      enqueueSnackbar(message || 'Erro ao gerar relatorio', { variant: 'error' });
    }
  }

  async function handleExportarExcel() {
    try {
      await exportarExcelMut.mutateAsync(buildRequest());
      enqueueSnackbar('Excel exportado com sucesso', { variant: 'success' });
    } catch {
      enqueueSnackbar('Erro ao exportar Excel', { variant: 'error' });
    }
  }

  async function handleExportarPdf() {
    try {
      await exportarPdfMut.mutateAsync(buildRequest());
      enqueueSnackbar('PDF exportado com sucesso', { variant: 'success' });
    } catch {
      enqueueSnackbar('Erro ao exportar PDF', { variant: 'error' });
    }
  }

  const columns: GridColDef[] = resultado
    ? resultado.colunas.map(col => ({
        field: col,
        headerName: col,
        flex: 1,
        minWidth: 120,
        valueFormatter: (value) => (isIsoDateString(value) ? formatDateToDisplay(value) : value),
      }))
    : [];

  const rows = resultado
    ? resultado.linhas.map((row, i) => ({ id: i, ...row }))
    : [];

  return (
    <Box>
      <PageHeader title="Relatorios" />
      <RelatorioFilters
        tipo={tipo} onTipoChange={setTipo}
        mes={mes} onMesChange={setMes}
        ano={ano} onAnoChange={setAno}
        mesFim={mesFim} onMesFimChange={setMesFim}
        anoFim={anoFim} onAnoFimChange={setAnoFim}
        setorId={setorId} onSetorIdChange={setSetorId}
        guardaId={guardaId} onGuardaIdChange={setGuardaId}
        onGerar={handleGerar}
        onExportarExcel={handleExportarExcel}
        onExportarPdf={handleExportarPdf}
        isLoading={gerarMut.isPending || exportarExcelMut.isPending || exportarPdfMut.isPending}
      />
      {resultado && (
        <>
          <Typography variant="h6" mt={3} mb={2}>{resultado.tituloRelatorio}</Typography>
          {rows.length === 0 ? (
            <Typography color="text.secondary">Não há registros</Typography>
          ) : (
            <DataGrid
              rows={rows} columns={columns} autoHeight
              pageSizeOptions={[10, 25, 50]}
              initialState={{ pagination: { paginationModel: { pageSize: 25 } } }}
            />
          )}
        </>
      )}
    </Box>
  );
}
