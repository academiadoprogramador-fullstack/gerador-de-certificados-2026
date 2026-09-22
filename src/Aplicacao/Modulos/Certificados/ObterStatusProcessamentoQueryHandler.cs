using FluentResults;
using GeradorCertificadosOnline.Aplicacao.Modulos.Certificados.DTOs;
using GeradorCertificadosOnline.Aplicacao.Modulos.Certificados.Util;
using GeradorCertificadosOnline.Dominio.Modulos.Certificados;
using MediatR;

namespace GeradorCertificadosOnline.Aplicacao.Modulos.Certificados;

public sealed record ObterStatusProcessamentoQuery(Guid CursoId)
    : IRequest<Result<StatusProcessamentoDto>>;

public sealed class ObterStatusProcessamentoQueryHandler(
    IRepositorioProcessamentoCertificados repositorioProcessamento)
    : IRequestHandler<ObterStatusProcessamentoQuery, Result<StatusProcessamentoDto>>
{
    public async Task<Result<StatusProcessamentoDto>> Handle(
        ObterStatusProcessamentoQuery query,
        CancellationToken cancellationToken
    )
    {
        var processamento = await repositorioProcessamento.SelecionarPorCursoAsync(
            query.CursoId,
            cancellationToken);

        return processamento is null
            ? Result.Fail(ErrosDeCertificado.ProcessamentoNaoEncontrado())
            : Result.Ok(StatusProcessamentoDto.From(processamento));
    }
}
