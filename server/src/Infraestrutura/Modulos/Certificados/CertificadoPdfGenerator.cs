using GeradorCertificadosOnline.Dominio.Modulos.Certificados;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace GeradorCertificadosOnline.Infraestrutura.Modulos.Certificados;

/// <summary>
/// Template fixo no código; templates customizáveis ficam fora do escopo da V1.
/// </summary>
public sealed class CertificadoPdfGenerator : ICertificadoPdfGenerator
{
    private const string AzulMarinho = "#101B36";
    private const string Indigo = "#4338CA";
    private const string Ciano = "#67E8F9";
    private const string Cinza = "#64748B";
    private const string Fundo = "#F4F6FB";

    public byte[] Gerar(string nomeAluno, string nomeCurso, int cargaHoraria, DateOnly dataConclusao)
    {
        var documento = Document.Create(container =>
        {
            container.Page(page =>
            {
                page.Size(PageSizes.A4.Landscape());
                page.Margin(1.4f, Unit.Centimetre);
                page.PageColor(Fundo);
                page.DefaultTextStyle(x => x.FontFamily("Lato").FontSize(12).FontColor(AzulMarinho));

                page.Content()
                    .Border(1)
                    .BorderColor("#CBD5E1")
                    .Background("#FFFFFF")
                    .Padding(10)
                    .Border(3)
                    .BorderColor(AzulMarinho)
                    .PaddingHorizontal(42)
                    .PaddingVertical(28)
                    .Column(column =>
                    {
                        column.Spacing(16);

                        column.Item().Row(row =>
                        {
                            row.RelativeItem().AlignLeft().DefaultTextStyle(x => x.FontSize(17)).Text(text =>
                            {
                                text.Span("Certifica").FontColor(AzulMarinho).Bold();
                                text.Span("+").FontColor(Indigo).Bold();
                            });

                            row.RelativeItem().AlignRight().AlignMiddle()
                                .Text("FORMAÇÃO QUE MERECE RECONHECIMENTO")
                                .FontSize(9).Bold().FontColor(Indigo);
                        });

                        column.Item().PaddingTop(8).AlignCenter()
                            .Width(54).Height(3).Background(Ciano);

                        column.Item().PaddingTop(10).AlignCenter()
                            .Background(AzulMarinho)
                            .PaddingHorizontal(24)
                            .PaddingVertical(10)
                            .Text("CERTIFICADO DE CONCLUSÃO")
                            .FontSize(25).Bold().FontColor("#FFFFFF");

                        column.Item().PaddingTop(12).AlignCenter().Text("Certificamos que")
                            .FontSize(13).FontColor(Cinza);

                        column.Item().PaddingTop(6).AlignCenter()
                            .Text(nomeAluno)
                            .FontSize(32).Bold().FontColor(Indigo);

                        column.Item().PaddingTop(4).AlignCenter().DefaultTextStyle(x => x.FontSize(14)).Text(text =>
                        {
                            text.Span("concluiu com êxito o curso ").FontColor(Cinza);
                            text.Span(nomeCurso).Bold().FontColor(AzulMarinho);
                        });

                        column.Item().PaddingTop(14).Row(row =>
                        {
                            row.RelativeItem().AlignCenter().Column(item =>
                            {
                                item.Item().AlignCenter().Text("CARGA HORÁRIA").FontSize(8).Bold()
                                    .FontColor(Cinza);
                                item.Item().PaddingTop(3).AlignCenter().Text($"{cargaHoraria} horas")
                                    .FontSize(12).Bold().FontColor(AzulMarinho);
                            });

                            row.ConstantItem(1).Height(28).Background("#E2E8F0");

                            row.RelativeItem().AlignCenter().Column(item =>
                            {
                                item.Item().AlignCenter().Text("DATA DE CONCLUSÃO").FontSize(8).Bold()
                                    .FontColor(Cinza);
                                item.Item().PaddingTop(3).AlignCenter().Text(dataConclusao.ToString("dd/MM/yyyy"))
                                    .FontSize(12).Bold().FontColor(AzulMarinho);
                            });
                        });

                        column.Item().PaddingTop(8).AlignCenter()
                            .Text("Cada conclusão merece reconhecimento.")
                            .FontSize(10).FontColor(Cinza).Italic();
                    });
            });
        });

        return documento.GeneratePdf();
    }
}
