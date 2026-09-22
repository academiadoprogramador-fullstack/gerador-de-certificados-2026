using FluentResults;
using GeradorCertificadosOnline.Aplicacao.Modulos.Certificados.DTOs;
using GeradorCertificadosOnline.Aplicacao.Modulos.Cursos.Util;
using GeradorCertificadosOnline.Dominio.Modulos.Certificados;
using GeradorCertificadosOnline.Dominio.Modulos.Cursos;
using MediatR;

namespace GeradorCertificadosOnline.Aplicacao.Modulos.Certificados;

public sealed record ListarCertificadosQuery(Guid CursoId)
    : IRequest<Result<IReadOnlyList<CertificadoDto>>>;
public sealed class ListarCertificadosQueryHandler(
    IRepositorioCurso repositorioCurso,
    IRepositorioProcessamentoCertificados repositorioProcessamento)
    : IRequestHandler<ListarCertificadosQuery, Result<IReadOnlyList<CertificadoDto>>>
{
    public async Task<Result<IReadOnlyList<CertificadoDto>>> Handle(
        ListarCertificadosQuery query,
        CancellationToken cancellationToken
    )
    {
        if (!await repositorioCurso.ExisteAsync(query.CursoId, cancellationToken))
            return Result.Fail(ErrosDeCurso.NaoEncontrado());

        var processamento = await repositorioProcessamento.SelecionarPorCursoAsync(
            query.CursoId,
            cancellationToken);

        if (processamento is null)
            return Result.Ok<IReadOnlyList<CertificadoDto>>([]);

        IReadOnlyList<CertificadoDto> dtos = processamento
            .Certificados
            .OrderBy(c => c.NomeAluno)
            .Select(CertificadoDto.From)
            .ToList();

        return Result.Ok(dtos);
    }
}
