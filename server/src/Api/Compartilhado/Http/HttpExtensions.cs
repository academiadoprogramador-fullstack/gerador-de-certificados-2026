using Microsoft.AspNetCore.Mvc;

namespace GeradorCertificadosOnline.Api.Compartilhado.Http;

public static class HttpExtensions
{
    public static IServiceCollection AddHttpServices(this IServiceCollection services)
    {
        services.AddControllers(
            options => options.SuppressImplicitRequiredAttributeForNonNullableReferenceTypes = true)
            .ConfigureApiBehaviorOptions(options =>
            {
                foreach (var status in new[] { 400, 401, 403, 404, 409 })
                    options.ClientErrorMapping[status].Link = ProblemDetailsTypes.ObterPorStatus(status);

                options.InvalidModelStateResponseFactory = context =>
                {
                    var problem = new ValidationProblemDetails(context.ModelState)
                    {
                        Status = 400,
                        Title = "Requisição Inválida"
                    };
                    problem.Enriquecer(context.HttpContext);

                    var response = new BadRequestObjectResult(problem);
                    response.ContentTypes.Add("application/problem+json");

                    return response;
                };
            });

        services.AddProblemDetails(options => options.CustomizeProblemDetails = context =>
        {
            var status = context.ProblemDetails.Status ?? 500;
            context.ProblemDetails.Status = status;
            context.ProblemDetails.Enriquecer(context.HttpContext);
        });

        return services;
    }
}
