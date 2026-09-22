using FluentResults;
using GeradorCertificadosOnline.Aplicacao.Compartilhado;

namespace GeradorCertificadosOnline.Aplicacao.Modulos.Certificados.Util;

public static class ErrosDeCertificado
{
    public static Error ProcessamentoNaoEncontrado()
    {
        return Erros.Criar(
            TipoErro.NaoEncontrado,
            "O processamento do curso não foi encontrado.");
    }

    public static Error EmAndamento()
    {
        return Erros.Criar(
            TipoErro.Conflito,
            "Curso já possui um processamento em andamento.");
    }

    public static Error ZipIndisponivel()
    {
        return Erros.Criar(
            TipoErro.Conflito,
            "O processamento dos certificados ainda não foi concluído.");
    }
}
