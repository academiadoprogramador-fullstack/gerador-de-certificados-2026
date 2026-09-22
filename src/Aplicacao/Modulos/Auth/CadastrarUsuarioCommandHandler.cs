using FluentResults;
using GeradorCertificadosOnline.Aplicacao.Compartilhado;
using GeradorCertificadosOnline.Dominio.Compartilhado;
using GeradorCertificadosOnline.Dominio.Compartilhado.Auth;
using MediatR;

namespace GeradorCertificadosOnline.Aplicacao.Modulos.Auth;

public sealed record CadastrarUsuarioCommand(
    string Email,
    string Senha) : IRequest<Result<UsuarioDto>>;

public sealed class CadastrarUsuarioCommandHandler(
    IGerenciadorIdentidade identidade)
    : IRequestHandler<CadastrarUsuarioCommand, Result<UsuarioDto>>
{
    public async Task<Result<UsuarioDto>> Handle(
        CadastrarUsuarioCommand command,
        CancellationToken cancellationToken)
    {
        var erros = new List<ErroValidacao>();

        if (string.IsNullOrWhiteSpace(command.Email))
            erros.Add(new("email", "Email é obrigatório."));

        if (string.IsNullOrWhiteSpace(command.Senha))
            erros.Add(new("senha", "Senha é obrigatória."));

        if (erros.Count > 0)
            return Result.Fail<UsuarioDto>(Erros.Validacao(erros));

        try
        {
            return Result.Ok(await identidade.CadastrarAsync(
                Guid.CreateVersion7(),
                command.Email,
                command.Senha,
                cancellationToken));
        }
        catch (ValidacaoDeIdentidadeException exception)
        {
            return Result.Fail<UsuarioDto>(Erros.Validacao(
                exception.Erros.Select(e => new ErroValidacao(e.Campo, e.Mensagem))));
        }
        catch (ConflitoDeIdentidadeException exception)
        {
            return Result.Fail<UsuarioDto>(
                Erros.Criar(TipoErro.Conflito, exception.Message));
        }
    }
}
