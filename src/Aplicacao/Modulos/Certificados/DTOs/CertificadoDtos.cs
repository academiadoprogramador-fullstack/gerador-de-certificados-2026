using GeradorCertificadosOnline.Dominio.Modulos.Certificados;

namespace GeradorCertificadosOnline.Aplicacao.Modulos.Certificados.DTOs;

public sealed record SolicitacaoCertificadosDto(
    Guid CursoId,
    int TotalAlunos,
    string Status);

public sealed record StatusProcessamentoDto(
    Guid CursoId,
    string Status,
    int TotalCertificados,
    int Gerados,
    int Falhas,
    bool ZipDisponivel,
    DateTimeOffset IniciadoEm,
    DateTimeOffset? ConcluidoEm)
{
    public static StatusProcessamentoDto From(ProcessamentoCertificados processamento)
    {
        return new StatusProcessamentoDto(
            processamento.CursoId,
            processamento.Status.ToString(),
            processamento.TotalCertificados,
            processamento.Gerados,
            processamento.Falhas,
            processamento.CaminhoZip is not null,
            processamento.IniciadoEm,
            processamento.ConcluidoEm);
    }
}

public sealed record CertificadoDto(
    Guid Id,
    string NomeAluno,
    string StatusGeracao,
    DateTimeOffset? GeradoEm)
{
    public static CertificadoDto From(Certificado certificado)
    {
        return new CertificadoDto(
            certificado.Id,
            certificado.NomeAluno,
            certificado.StatusGeracao.ToString(),
            certificado.GeradoEm);
    }
}

public sealed record ArquivoCertificadosDto(
    Stream Conteudo,
    string Nome,
    string ContentType);
