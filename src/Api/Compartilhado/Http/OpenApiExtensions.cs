using Microsoft.AspNetCore.Authorization;
using Microsoft.OpenApi;

namespace GeradorCertificadosOnline.Api.Compartilhado.Http;

/// <summary>
/// Registra a documentação OpenAPI da API e representa a política global de autenticação.
/// </summary>
public static class OpenApiExtensions
{
    /// <summary>
    /// Configura título, descrição, esquema Bearer e segurança das operações protegidas.
    /// </summary>
    public static IServiceCollection AddOpenApiServices(this IServiceCollection services)
    {
        services.AddOpenApi(options =>
        {
            options.AddDocumentTransformer((document, _, _) =>
            {
                document.Info = new OpenApiInfo
                {
                    Title = "Gerador de Certificados API",
                    Version = "v1",
                    Description = "API para cadastro de cursos e geração assíncrona de certificados em PDF e ZIP."
                };

                var components = document.Components ??= new OpenApiComponents();
                components.SecuritySchemes ??= new Dictionary<string, IOpenApiSecurityScheme>();
                components.SecuritySchemes["Bearer"] = new OpenApiSecurityScheme
                {
                    Type = SecuritySchemeType.Http,
                    Scheme = "bearer",
                    BearerFormat = "JWT",
                    In = ParameterLocation.Header,
                    Description = "Informe o token obtido em POST /auth/login."
                };
                return Task.CompletedTask;
            });
            options.AddOperationTransformer((operation, context, _) =>
            {
                // Inclui a política global, mesmo quando a action não possui Authorize explícito.
                if (!context.Description.ActionDescriptor.EndpointMetadata.OfType<IAllowAnonymous>().Any())
                {
                    operation.Security ??= [];
                    operation.Security.Add(new OpenApiSecurityRequirement
                    {
                        [new OpenApiSecuritySchemeReference("Bearer", context.Document)] = []
                    });
                }
                return Task.CompletedTask;
            });
        });
        return services;
    }
}
