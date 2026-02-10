using ClosedXML.Excel;
using EscalaGcm.Application.DTOs.Relatorios;

namespace EscalaGcm.Infrastructure.Services;

public static class ExcelReportGenerator
{
    public static byte[] Generate(RelatorioResult result)
    {
        using var workbook = new XLWorkbook();
        var ws = workbook.Worksheets.Add("Relatorio");

        // Title row
        ws.Cell(1, 1).Value = result.TituloRelatorio;
        ws.Cell(1, 1).Style.Font.Bold = true;
        ws.Cell(1, 1).Style.Font.FontSize = 14;

        // Header row
        for (int c = 0; c < result.Colunas.Count; c++)
        {
            var cell = ws.Cell(3, c + 1);
            cell.Value = result.Colunas[c];
            cell.Style.Font.Bold = true;
            cell.Style.Fill.BackgroundColor = XLColor.LightGray;
        }

        // Data rows
        for (int r = 0; r < result.Linhas.Count; r++)
        {
            for (int c = 0; c < result.Colunas.Count; c++)
            {
                var col = result.Colunas[c];
                ws.Cell(r + 4, c + 1).Value = result.Linhas[r].GetValueOrDefault(col, "");
            }
        }

        ws.Columns().AdjustToContents();

        using var stream = new MemoryStream();
        workbook.SaveAs(stream);
        return stream.ToArray();
    }
}
