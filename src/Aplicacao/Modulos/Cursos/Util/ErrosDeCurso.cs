using FluentResults;
using GeradorCertificadosOnline.Aplicacao.Compartilhado;

namespace GeradorCertificadosOnline.Aplicacao.Modulos.Cursos.Util;

public static class ErrosDeCurso
{
    public static Error NaoEncontrado()
    {
        return Erros.Criar(TipoErro.NaoEncontrado, "O curso não foi encontrado.");
    }
}
