using FluentResults;
using GeradorCertificadosOnline.Aplicacao.Modulos.Cursos.DTOs;
using GeradorCertificadosOnline.Aplicacao.Modulos.Cursos.Util;
using GeradorCertificadosOnline.Dominio.Modulos.Cursos;
using MediatR;

namespace GeradorCertificadosOnline.Aplicacao.Modulos.Cursos;

public sealed record ObterCursoPorIdQuery(Guid CursoId) : IRequest<Result<CursoDto>>;

public sealed class ObterCursoPorIdQueryHandler(IRepositorioCurso repositorioCurso)
    : IRequestHandler<ObterCursoPorIdQuery, Result<CursoDto>>
{
    public async Task<Result<CursoDto>> Handle(
        ObterCursoPorIdQuery query,
        CancellationToken cancellationToken)
    {
        var curso = await repositorioCurso.SelecionarPorIdAsync(
            query.CursoId,
            cancellationToken);

        return curso is null ? Result.Fail(ErrosDeCurso.NaoEncontrado())
            : Result.Ok(CursoDto.From(curso));
    }
}
