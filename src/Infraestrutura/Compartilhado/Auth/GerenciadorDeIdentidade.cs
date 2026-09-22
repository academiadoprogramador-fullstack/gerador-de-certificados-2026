using GeradorCertificadosOnline.Dominio.Compartilhado.Auth;
using Microsoft.AspNetCore.Identity;

namespace GeradorCertificadosOnline.Infraestrutura.Compartilhado.Auth;

public sealed class GerenciadorDeIdentidade(
    UserManager<IdentityUser<Guid>> userManager)
    : IGerenciadorIdentidade
{
    public async Task<UsuarioDto> CadastrarAsync(
        Guid id,
        string email,
        string senha,
        CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var usuario = new IdentityUser<Guid>
        {
            Id = id,
            UserName = email,
            Email = email,
            LockoutEnabled = true
        };

        var resultado = await userManager.CreateAsync(usuario, senha);
        if (resultado.Succeeded)
            return new UsuarioDto(usuario.Id, usuario.Email!);

        if (resultado.Errors.Any(e =>
                e.Code is "DuplicateEmail" or "DuplicateUserName"))
        {
            throw new ConflitoDeIdentidadeException("O email já está cadastrado.");
        }

        throw new ValidacaoDeIdentidadeException(
            resultado.Errors
                .Select(e => new ErroDeIdentidade(
                    e.Code.StartsWith("Password", StringComparison.Ordinal)
                        ? "senha"
                        : "email",
                    e.Description))
                .ToArray());
    }

    public async Task<UsuarioDto?> ChecarValidadeDeSenhaAsync(
        string email,
        string senha,
        CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var usuario = await userManager.FindByEmailAsync(email);
        if (usuario is null || await userManager.IsLockedOutAsync(usuario))
            return null;

        if (!await userManager.CheckPasswordAsync(usuario, senha))
        {
            await userManager.AccessFailedAsync(usuario);
            return null;
        }

        await userManager.ResetAccessFailedCountAsync(usuario);
        return new UsuarioDto(usuario.Id, usuario.Email!);
    }
}
