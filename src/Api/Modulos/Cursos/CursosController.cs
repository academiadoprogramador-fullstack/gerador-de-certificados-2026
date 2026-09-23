using GeradorCertificadosOnline.Api.Compartilhado.Http;
using GeradorCertificadosOnline.Aplicacao.Modulos.Cursos;
using GeradorCertificadosOnline.Aplicacao.Modulos.Cursos.DTOs;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace GeradorCertificadosOnline.Api.Modulos.Cursos;

[ApiController]
[Route("cursos")]
[ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized, "application/problem+json")]
public sealed class CursosController(IMediator mediator) : ControllerBase
{
    [HttpPost(Name = "CriarCurso")]
    [Tags("Cursos")]
    [EndpointSummary("Cadastra um curso")]
    [EndpointDescription("Cria um curso e retorna sua representação com uma URL para consulta posterior.")]
    [ProducesResponseType<CursoDto>(201)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest, "application/problem+json")]
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
    [Tags("Cursos")]
    [EndpointSummary("Consulta um curso")]
    [EndpointDescription("Retorna os dados do curso identificado por cursoId.")]
    [ProducesResponseType<CursoDto>(200)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound, "application/problem+json")]
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
