using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace GeradorCertificadosOnline.Api.Compartilhado.Http;

public static class ProblemDetailsExtensions
{
    public static void Enriquecer(this ProblemDetails problem, HttpContext context)
    {
        var status = problem.Status ?? 500;
        problem.Type = ProblemDetailsTypes.ObterPorStatus(status);

        if (status == 500)
        {
            problem.Title = "Erro Interno do Servidor";
            problem.Detail = null;
        }

        problem.Extensions["traceId"] = Activity.Current?.Id ?? context.TraceIdentifier;
    }
}
