namespace GeradorCertificadosOnline.Dominio.Compartilhado.Auth;

public sealed record UsuarioDto(Guid Id, string Email);

public sealed record TokenDto(
    Guid UsuarioId,
    string AccessToken,
    DateTime DataExpiracaoEmUtc);

public interface IEmissorTokens
{
    TokenDto CriarToken(Guid usuarioId, string email);
}

public interface IGerenciadorIdentidade
{
    Task<UsuarioDto> CadastrarAsync(
        Guid id,
        string email,
        string senha,
        CancellationToken cancellationToken);

    Task<UsuarioDto?> ChecarValidadeDeSenhaAsync(
        string email,
        string senha,
        CancellationToken cancellationToken);
}

public sealed record ErroDeIdentidade(string Campo, string Mensagem);

public sealed class ValidacaoDeIdentidadeException(
    IReadOnlyCollection<ErroDeIdentidade> erros)
    : Exception("Os dados de identidade são inválidos.")
{
    public IReadOnlyCollection<ErroDeIdentidade> Erros { get; } = erros;
}

public sealed class ConflitoDeIdentidadeException(string mensagem)
    : Exception(mensagem);
