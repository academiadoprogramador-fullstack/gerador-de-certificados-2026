using FluentResults;
using GeradorCertificadosOnline.Aplicacao.Compartilhado;
using Microsoft.AspNetCore.Mvc;

namespace GeradorCertificadosOnline.Api.Compartilhado.Http;

public static class ControllerBaseExtensions
{
    public static ActionResult ProblemDetails(this ControllerBase controller, ResultBase resultado)
    {
        var erro = resultado.Errors.FirstOrDefault();
        var tipo = erro?.Metadata.GetValueOrDefault(nameof(TipoErro)) as TipoErro?;

        var status = tipo switch
        {
            TipoErro.Validacao => 400,
            TipoErro.NaoAutenticado => 401,
            TipoErro.NaoAutorizado => 403,
            TipoErro.NaoEncontrado => 404,
            TipoErro.Conflito => 409,
            _ => 500
        };

        ProblemDetails problem;

        if (tipo == TipoErro.Validacao)
        {
            var erros = resultado.Errors
                .GroupBy(e => e.Metadata.GetValueOrDefault("Campo")?.ToString() ?? string.Empty)
                .ToDictionary(g => g.Key, g => g.Select(e => e.Message).ToArray());
            problem = new ValidationProblemDetails(erros)
            {
                Title = "Requisição Inválida"
            };
        }
        else
        {
            problem = new ProblemDetails
            {
                Title = status switch
                {
                    401 => "Não Autenticado",
                    403 => "Acesso Negado",
                    404 => "Recurso Não Encontrado",
                    409 => "Conflito",
                    _ => "Erro Interno do Servidor"
                },
                Detail = status == 500 ? null : erro?.Message
            };
        }

        problem.Status = status;
        problem.Enriquecer(controller.HttpContext);

        var response = new ObjectResult(problem)
        {
            StatusCode = status
        };
        response.ContentTypes.Add("application/problem+json");

        return response;
    }
}
