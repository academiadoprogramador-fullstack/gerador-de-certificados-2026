using FluentResults;
using GeradorCertificadosOnline.Dominio.Compartilhado;

namespace GeradorCertificadosOnline.Aplicacao.Compartilhado;

public enum TipoErro { Validacao, NaoEncontrado, Conflito, NaoAutenticado, NaoAutorizado }

public static class Erros
{
    public static Error Criar(TipoErro tipo, string mensagem)
    {
        return new Error(mensagem).WithMetadata(nameof(TipoErro), tipo);
    }

    public static IEnumerable<Error> Validacao(IEnumerable<ErroValidacao> erros)
    {
        return erros.Select(e => Criar(TipoErro.Validacao, e.Mensagem)
            .WithMetadata("Campo", e.Campo));
    }
}
