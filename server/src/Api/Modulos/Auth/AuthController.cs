using GeradorCertificadosOnline.Api.Compartilhado.Http;
using GeradorCertificadosOnline.Aplicacao.Modulos.Auth;
using GeradorCertificadosOnline.Dominio.Compartilhado.Auth;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace GeradorCertificadosOnline.Api.Modulos.Auth;

[ApiController]
[Route("auth")]
public sealed class AuthController(IMediator mediator) : ControllerBase
{
    [AllowAnonymous]
    [HttpPost("login", Name = "Login")]
    [Tags("Autenticação")]
    [EndpointSummary("Autentica um usuário")]
    [EndpointDescription("Valida as credenciais no ASP.NET Core Identity e devolve um token JWT para as rotas protegidas.")]
    [ProducesResponseType<TokenDto>(200)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest, "application/problem+json")]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized, "application/problem+json")]
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
    [Tags("Autenticação")]
    [EndpointSummary("Cadastra um usuário")]
    [EndpointDescription("Cria um usuário no ASP.NET Core Identity. O e-mail deve ser único e a senha deve atender à política configurada.")]
    [ProducesResponseType<UsuarioDto>(201)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest, "application/problem+json")]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status409Conflict, "application/problem+json")]
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
