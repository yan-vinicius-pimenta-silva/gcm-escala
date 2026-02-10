using EscalaGcm.Application.DTOs.Relatorios;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace EscalaGcm.Infrastructure.Services;

public static class PdfReportGenerator
{
    public static byte[] Generate(RelatorioResult report)
    {
        QuestPDF.Settings.License = LicenseType.Community;

        return Document.Create(container =>
        {
            container.Page(page =>
            {
                page.Margin(24);
                page.Size(PageSizes.A4.Landscape());
                page.DefaultTextStyle(x => x.FontSize(10));

                page.Header()
                    .Text(report.TituloRelatorio)
                    .SemiBold()
                    .FontSize(16)
                    .FontColor(Colors.Blue.Darken2);

                page.Content().PaddingTop(12).Table(table =>
                {
                    table.ColumnsDefinition(columns =>
                    {
                        foreach (var _ in report.Colunas)
                            columns.RelativeColumn();
                    });

                    table.Header(header =>
                    {
                        foreach (var col in report.Colunas)
                        {
                            header.Cell()
                                .Background(Colors.Grey.Lighten3)
                                .Border(1)
                                .BorderColor(Colors.Grey.Lighten1)
                                .Padding(6)
                                .Text(col)
                                .SemiBold();
                        }
                    });

                    foreach (var linha in report.Linhas)
                    {
                        foreach (var col in report.Colunas)
                        {
                            linha.TryGetValue(col, out var value);

                            table.Cell()
                                .Border(1)
                                .BorderColor(Colors.Grey.Lighten2)
                                .Padding(4)
                                .Text(value ?? string.Empty);
                        }
                    }
                });

                page.Footer().AlignRight().Text(text =>
                {
                    text.Span("Página ");
                    text.CurrentPageNumber();
                    text.Span(" de ");
                    text.TotalPages();
                });
            });
        }).GeneratePdf();
    }
}
