using FluentResults;
using GeradorCertificadosOnline.Aplicacao.Modulos.Cursos.DTOs;
using GeradorCertificadosOnline.Dominio.Modulos.Cursos;
using MediatR;

namespace GeradorCertificadosOnline.Aplicacao.Modulos.Cursos;

public sealed record ListarCursosQuery(Guid UsuarioId) : IRequest<Result<IReadOnlyList<CursoDto>>>;

public sealed class ListarCursosQueryHandler(IRepositorioCurso repositorioCurso)
    : IRequestHandler<ListarCursosQuery, Result<IReadOnlyList<CursoDto>>>
{
    public async Task<Result<IReadOnlyList<CursoDto>>> Handle(
        ListarCursosQuery query,
        CancellationToken cancellationToken)
    {
        var cursos = await repositorioCurso.SelecionarTodosAsync(query.UsuarioId, cancellationToken);

        IReadOnlyList<CursoDto> dtos = cursos
            .Select(CursoDto.From)
            .ToList();

        return Result.Ok(dtos);
    }
}
