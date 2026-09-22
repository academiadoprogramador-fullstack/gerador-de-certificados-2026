using GeradorCertificadosOnline.Api.Compartilhado.Http;
using GeradorCertificadosOnline.Aplicacao.Modulos.Cursos;
using GeradorCertificadosOnline.Aplicacao.Modulos.Cursos.DTOs;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace GeradorCertificadosOnline.Api.Modulos.Cursos;

[ApiController]
[Route("cursos")]
[ProducesResponseType<ProblemDetails>(401)]
public sealed class CursosController(IMediator mediator) : ControllerBase
{
    [HttpPost(Name = "CriarCurso")]
    [ProducesResponseType<CursoDto>(201)]
    [ProducesResponseType<ValidationProblemDetails>(400)]
    public async Task<ActionResult<CursoDto>> Cadastrar(
        CriarCursoRequest request,
        CancellationToken cancellationToken)
    {
        var command = new CadastrarCursoCommand(
            request.Nome,
            request.CargaHoraria,
            request.DataConclusao);

        var resultado = await mediator.Send(
            command,
            cancellationToken);

        if (resultado.IsFailed)
            return this.ProblemDetails(resultado);

        var curso = resultado.Value;

        return CreatedAtAction(
            nameof(ObterPorId),
            new { cursoId = curso.Id },
            curso);
    }

    [HttpGet("{cursoId:guid}", Name = "ObterCursoPorId")]
    [ProducesResponseType<CursoDto>(200)]
    [ProducesResponseType<ProblemDetails>(404)]
    public async Task<ActionResult<CursoDto>> ObterPorId(
        Guid cursoId,
        CancellationToken cancellationToken)
    {
        var resultado = await mediator.Send(
            new ObterCursoPorIdQuery(cursoId),
            cancellationToken);

        if (resultado.IsFailed)
            return this.ProblemDetails(resultado);

        return Ok(resultado.Value);
    }
}
