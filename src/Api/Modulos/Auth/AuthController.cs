using GeradorCertificadosOnline.Api.Compartilhado.Http;
using GeradorCertificadosOnline.Aplicacao.Modulos.Auth;
using GeradorCertificadosOnline.Dominio.Compartilhado.Auth;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GeradorCertificadosOnline.Api.Modulos.Auth;

[ApiController]
[Route("auth")]
public sealed class AuthController(IMediator mediator) : ControllerBase
{
    [AllowAnonymous]
    [HttpPost("login", Name = "Login")]
    [ProducesResponseType<TokenDto>(200)]
    [ProducesResponseType<ValidationProblemDetails>(400)]
    [ProducesResponseType<ProblemDetails>(401)]
    public async Task<ActionResult<TokenDto>> Login(
        LoginRequest request,
        CancellationToken cancellationToken)
    {
        var command = new AutenticarUsuarioCommand(
            request.Email,
            request.Senha);

        var resultado = await mediator.Send(
            command,
            cancellationToken);

        if (resultado.IsFailed)
            return this.ProblemDetails(resultado);

        return Ok(resultado.Value);
    }

    [AllowAnonymous]
    [HttpPost("cadastro", Name = "CadastrarUsuario")]
    [ProducesResponseType<UsuarioDto>(201)]
    [ProducesResponseType<ValidationProblemDetails>(400)]
    [ProducesResponseType<ProblemDetails>(409)]
    public async Task<ActionResult<UsuarioDto>> Cadastro(
        CadastroRequest request,
        CancellationToken cancellationToken)
    {
        var command = new CadastrarUsuarioCommand(
            request.Email,
            request.Senha);

        var resultado = await mediator.Send(
            command,
            cancellationToken);

        if (resultado.IsFailed)
            return this.ProblemDetails(resultado);

        return StatusCode(StatusCodes.Status201Created, resultado.Value);
    }
}
