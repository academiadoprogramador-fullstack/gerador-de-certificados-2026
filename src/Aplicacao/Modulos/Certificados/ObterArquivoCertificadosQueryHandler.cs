using FluentResults;
using GeradorCertificadosOnline.Aplicacao.Modulos.Certificados.DTOs;
using GeradorCertificadosOnline.Aplicacao.Modulos.Certificados.Util;
using GeradorCertificadosOnline.Dominio.Modulos.Certificados;
using MediatR;

namespace GeradorCertificadosOnline.Aplicacao.Modulos.Certificados;

public sealed record ObterArquivoCertificadosQuery(Guid CursoId)
    : IRequest<Result<ArquivoCertificadosDto>>;

public sealed class ObterArquivoCertificadosQueryHandler(
    IRepositorioProcessamentoCertificados repositorioProcessamento,
    ICertificadoStorage certificadoStorage)
    : IRequestHandler<ObterArquivoCertificadosQuery, Result<ArquivoCertificadosDto>>
{
    public async Task<Result<ArquivoCertificadosDto>> Handle(
        ObterArquivoCertificadosQuery query,
        CancellationToken cancellationToken
    )
    {
        var processamento = await repositorioProcessamento.SelecionarPorCursoAsync(
            query.CursoId,
            cancellationToken);

        if (processamento is null)
            return Result.Fail(ErrosDeCertificado.ProcessamentoNaoEncontrado());

        if (!processamento.EstaFinalizado || processamento.CaminhoZip is null)
            return Result.Fail(ErrosDeCertificado.ZipIndisponivel());

        cancellationToken.ThrowIfCancellationRequested();

        return Result.Ok(new ArquivoCertificadosDto(
            certificadoStorage.AbrirLeitura(processamento.CaminhoZip),
            $"certificados-{query.CursoId}.zip",
            "application/zip")
        );
    }
}
