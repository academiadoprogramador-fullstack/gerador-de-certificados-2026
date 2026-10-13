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
    public byte[] Gerar(string nomeAluno, string nomeCurso, int cargaHoraria, DateOnly dataConclusao)
    {
        var documento = Document.Create(container =>
        {
            container.Page(page =>
            {
                page.Size(PageSizes.A4.Landscape());
                page.Margin(2, Unit.Centimetre);
                page.DefaultTextStyle(x => x.FontSize(16));

                page.Content().Column(column =>
                {
                    column.Spacing(20);
                    column.Item().AlignCenter().Text("Certificado de Conclusão")
                        .FontSize(32).Bold();
                    column.Item().AlignCenter().Text("Certificamos que").FontSize(18);
                    column.Item().AlignCenter().Text(nomeAluno)
                        .FontSize(26).Bold();
                    column.Item().AlignCenter().Text(text =>
                    {
                        text.Span("concluiu o curso ");
                        text.Span(nomeCurso).Bold();
                        text.Span($", com carga horária de {cargaHoraria} horas, em {dataConclusao:dd/MM/yyyy}.");
                    });
                });
            });
        });

        return documento.GeneratePdf();
    }
}
