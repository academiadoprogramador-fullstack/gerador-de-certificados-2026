using FluentResults;
using GeradorCertificadosOnline.Aplicacao.Compartilhado;
using GeradorCertificadosOnline.Dominio.Compartilhado;
using GeradorCertificadosOnline.Dominio.Compartilhado.Auth;
using MediatR;

namespace GeradorCertificadosOnline.Aplicacao.Modulos.Auth;

public sealed record AutenticarUsuarioCommand(
    string Email,
    string Senha) : IRequest<Result<TokenDto>>;

public sealed class AutenticarUsuarioCommandHandler(
    IGerenciadorIdentidade gerenciadorIdentidade,
    IEmissorTokens emissorTokens)
: IRequestHandler<AutenticarUsuarioCommand, Result<TokenDto>>
{
    public async Task<Result<TokenDto>> Handle(
        AutenticarUsuarioCommand command,
        CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        List<ErroValidacao> erros = [];

        if (string.IsNullOrWhiteSpace(command.Email))
            erros.Add(new("email", "Email é obrigatório."));

        if (string.IsNullOrWhiteSpace(command.Senha))
            erros.Add(new("senha", "Senha é obrigatória."));

        if (erros.Count > 0) return Result.Fail<TokenDto>(Erros.Validacao(erros));

        var usuario = await gerenciadorIdentidade.ChecarValidadeDeSenhaAsync(
            command.Email,
            command.Senha,
            cancellationToken);

        return usuario is null
            ? Result.Fail<TokenDto>(Erros.Criar(TipoErro.NaoAutenticado, "Credenciais inválidas"))
            : Result.Ok(emissorTokens.CriarToken(usuario.Id, usuario.Email));
    }
}
