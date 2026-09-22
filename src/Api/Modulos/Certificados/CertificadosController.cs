using GeradorCertificadosOnline.Api.Compartilhado.Http;
using GeradorCertificadosOnline.Aplicacao.Modulos.Certificados;
using GeradorCertificadosOnline.Aplicacao.Modulos.Certificados.DTOs;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace GeradorCertificadosOnline.Api.Modulos.Certificados;

[ApiController]
[Route("cursos/{cursoId:guid}")]
[ProducesResponseType<ProblemDetails>(401)]
[ProducesResponseType<ProblemDetails>(404)]
public sealed class CertificadosController(IMediator mediator) : ControllerBase
{
    [HttpPost("certificados", Name = "SolicitarGeracaoCertificados")]
    [ProducesResponseType<SolicitacaoCertificadosDto>(202)]
    [ProducesResponseType<ValidationProblemDetails>(400)]
    [ProducesResponseType<ProblemDetails>(409)]
    public async Task<ActionResult<SolicitacaoCertificadosDto>> SolicitarGeracao(
        Guid cursoId,
        SolicitarCertificadosRequest request,
        CancellationToken cancellationToken)
    {
        var alunos = request.Alunos?
            .Select(a => a is null ? null : new AlunoCommand(a.Nome))
            .ToList();

        var resultado = await mediator.Send(
            new SolicitarGeracaoCertificadosCommand(cursoId, alunos),
            cancellationToken);

        if (resultado.IsFailed)
            return this.ProblemDetails(resultado);

        var solicitacao = resultado.Value;

        return AcceptedAtAction(
            nameof(ObterStatus),
            new { cursoId },
            solicitacao);
    }

    [HttpGet("status", Name = "ConsultarStatusProcessamento")]
    [ProducesResponseType<StatusProcessamentoDto>(200)]
    public async Task<ActionResult<StatusProcessamentoDto>> ObterStatus(
        Guid cursoId,
        CancellationToken cancellationToken)
    {
        var resultado = await mediator.Send(
            new ObterStatusProcessamentoQuery(cursoId),
            cancellationToken);

        if (resultado.IsFailed)
            return this.ProblemDetails(resultado);

        return Ok(resultado.Value);
    }

    [HttpGet("certificados", Name = "ListarCertificadosCurso")]
    [ProducesResponseType<IReadOnlyList<CertificadoDto>>(200)]
    public async Task<ActionResult<IReadOnlyList<CertificadoDto>>> Listar(
        Guid cursoId,
        CancellationToken cancellationToken)
    {
        var resultado = await mediator.Send(
            new ListarCertificadosQuery(cursoId),
            cancellationToken);

        if (resultado.IsFailed)
            return this.ProblemDetails(resultado);

        return Ok(resultado.Value);
    }

    [HttpGet("certificados/download", Name = "DownloadCertificadosZip")]
    [ProducesResponseType(typeof(FileStreamResult), 200, "application/zip")]
    [ProducesResponseType<ProblemDetails>(409)]
    [ProducesResponseType<ProblemDetails>(500)]
    public async Task<IActionResult> Download(
        Guid cursoId,
        CancellationToken cancellationToken)
    {
        var resultado = await mediator.Send(
            new ObterArquivoCertificadosQuery(cursoId),
            cancellationToken);

        if (resultado.IsFailed)
            return this.ProblemDetails(resultado);

        var arquivo = resultado.Value;

        return File(arquivo.Conteudo, arquivo.ContentType, arquivo.Nome);
    }
}
