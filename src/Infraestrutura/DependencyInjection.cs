using GeradorCertificadosOnline.Dominio.Compartilhado.Auth;
using GeradorCertificadosOnline.Dominio.Modulos.Certificados;
using GeradorCertificadosOnline.Dominio.Modulos.Cursos;
using GeradorCertificadosOnline.Infraestrutura.Compartilhado.Auth;
using GeradorCertificadosOnline.Infraestrutura.Compartilhado.Orm;
using GeradorCertificadosOnline.Infraestrutura.Compartilhado.Storage;
using GeradorCertificadosOnline.Infraestrutura.Modulos.Certificados;
using GeradorCertificadosOnline.Infraestrutura.Modulos.Cursos;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using QuestPDF.Infrastructure;

namespace GeradorCertificadosOnline.Infraestrutura;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructureServices(
        this IServiceCollection services,
        IConfiguration configuration
    )
    {
        QuestPDF.Settings.License = LicenseType.Community;

        services.AddDbContext<CertificadosDbContext>(options =>
        {
            var connection = configuration.GetConnectionString("SqlServer");

            if (string.IsNullOrWhiteSpace(connection))
                throw new InvalidOperationException("Connection string \"SqlServer\" não configurada.");

            options.UseSqlServer(connection);
        });

        services.AddIdentityCore<IdentityUser<Guid>>(options =>
        {
            options.User.RequireUniqueEmail = true;
            options.SignIn.RequireConfirmedEmail = false;
            options.Password.RequiredLength = 8;
            options.Password.RequireDigit = true;
            options.Password.RequireNonAlphanumeric = true;
            options.Password.RequireUppercase = false;
            options.Password.RequireLowercase = false;
            options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);
            options.Lockout.MaxFailedAccessAttempts = 5;
            options.Lockout.AllowedForNewUsers = true;
        })
        .AddEntityFrameworkStores<CertificadosDbContext>();

        services.AddScoped<IGerenciadorIdentidade, GerenciadorDeIdentidade>();

        services.AddSingleton<ICertificadoPdfGenerator, CertificadoPdfGenerator>();
        services.AddSingleton<ICertificadoStorage, FileSystemCertificadoStorage>();

        services.AddScoped<IRepositorioCurso, RepositorioCursoEmOrm>();
        services.AddScoped<IRepositorioProcessamentoCertificados, RepositorioProcessamentoCertificadosEmOrm>();

        return services;
    }
}
