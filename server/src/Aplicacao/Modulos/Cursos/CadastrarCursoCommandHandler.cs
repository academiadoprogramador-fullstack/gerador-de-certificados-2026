using FluentResults;
using GeradorCertificadosOnline.Aplicacao.Compartilhado;
using GeradorCertificadosOnline.Aplicacao.Modulos.Cursos.DTOs;
using GeradorCertificadosOnline.Dominio.Modulos.Cursos;
using MediatR;

namespace GeradorCertificadosOnline.Aplicacao.Modulos.Cursos;

public sealed record CadastrarCursoCommand(
    string Nome,
    int CargaHoraria,
    DateOnly DataConclusao) : IRequest<Result<CursoDto>>;

public sealed class CadastrarCursoCommandHandler(IRepositorioCurso repositorioCurso)
    : IRequestHandler<CadastrarCursoCommand, Result<CursoDto>>
{
    public async Task<Result<CursoDto>> Handle(
        CadastrarCursoCommand command,
        CancellationToken cancellationToken
    )
    {
        var curso = new Curso(
            command.Nome,
            command.CargaHoraria,
            command.DataConclusao);

        var erros = curso.Validar();

        if (erros.Count > 0)
            return Result.Fail(Erros.Validacao(erros));

        await repositorioCurso.CadastrarAsync(curso, cancellationToken);

        return Result.Ok(CursoDto.From(curso));
    }
}
