using GeradorCertificadosOnline.Api.Compartilhado.Http;
using GeradorCertificadosOnline.Aplicacao.Modulos.Certificados;
using GeradorCertificadosOnline.Aplicacao.Modulos.Certificados.DTOs;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace GeradorCertificadosOnline.Api.Modulos.Certificados;

[ApiController]
[Route("cursos/{cursoId:guid}")]
[ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized, "application/problem+json")]
[ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound, "application/problem+json")]
public sealed class CertificadosController(IMediator mediator) : ControllerBase
{
    [HttpPost("certificados", Name = "SolicitarGeracaoCertificados")]
    [Tags("Certificados")]
    [EndpointSummary("Solicita a geração de certificados")]
    [EndpointDescription("Cria um lote de processamento e inicia a geração assíncrona. Use a URL retornada em Location para acompanhar o status.")]
    [ProducesResponseType<SolicitacaoCertificadosDto>(202)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest, "application/problem+json")]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status409Conflict, "application/problem+json")]
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
    [Tags("Certificados")]
    [EndpointSummary("Consulta o status da geração")]
    [EndpointDescription("Retorna o lote mais recente do curso, seus contadores e se o arquivo ZIP está disponível para download.")]
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
    [Tags("Certificados")]
    [EndpointSummary("Lista os certificados do curso")]
    [EndpointDescription("Retorna os certificados do lote mais recente ordenados pelo nome do aluno, incluindo o status individual de geração.")]
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
    [Tags("Certificados")]
    [EndpointSummary("Baixa o ZIP de certificados")]
    [EndpointDescription("Baixa o arquivo ZIP quando o processamento estiver concluído. Lotes concluídos com falhas também disponibilizam os PDFs gerados com sucesso.")]
    [ProducesResponseType(typeof(Stream), 200, "application/zip")]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status409Conflict, "application/problem+json")]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError, "application/problem+json")]
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
