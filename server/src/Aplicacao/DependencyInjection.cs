using GeradorCertificadosOnline.Aplicacao.Modulos.Certificados.Mensageria;
using MassTransit;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace GeradorCertificadosOnline.Aplicacao;

public static class DependencyInjection
{
    public static IServiceCollection AddApplicationServices(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddMediatR(config =>
            config.RegisterServicesFromAssembly(typeof(DependencyInjection).Assembly));

        var connection = configuration.GetConnectionString("RabbitMq");

        if (string.IsNullOrWhiteSpace(connection))
            throw new InvalidOperationException("Connection string \"RabbitMq\" não configurada.");

        services.AddMassTransit(config =>
        {
            config.SetKebabCaseEndpointNameFormatter();
            config.AddConsumer<GerarCertificadosConsumer>();

            config.UsingRabbitMq((context, rabbit) =>
            {
                rabbit.Host(new Uri(connection));

                rabbit.ReceiveEndpoint("gerar-certificados", endpoint =>
                {
                    endpoint.PrefetchCount = 4;
                    endpoint.ConcurrentMessageLimit = 2;
                    endpoint.UseMessageRetry(UsarRetryPolicy);

                    endpoint.ConfigureConsumer<GerarCertificadosConsumer>(context);
                });
            });
        });

        services.Configure<MassTransitHostOptions>(options =>
        {
            options.WaitUntilStarted = true;
            options.StartTimeout = TimeSpan.FromSeconds(30);
        });

        return services;
    }

    private static void UsarRetryPolicy(IRetryConfigurator retry)
    {
        retry.Intervals(
            TimeSpan.FromSeconds(5),
            TimeSpan.FromSeconds(15),
            TimeSpan.FromSeconds(30));
    }
}
