using GeradorCertificadosOnline.Dominio.Modulos.Certificados;
using GeradorCertificadosOnline.Dominio.Modulos.Cursos;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace GeradorCertificadosOnline.Infraestrutura.Compartilhado.Orm;

public class CertificadosDbContext(
    DbContextOptions<CertificadosDbContext> options)
    : IdentityUserContext<IdentityUser<Guid>, Guid>(options)
{
    public DbSet<Curso> Cursos
    {
        get { return Set<Curso>(); }
    }

    public DbSet<Certificado> Certificados
    {
        get { return Set<Certificado>(); }
    }

    public DbSet<ProcessamentoCertificados> ProcessamentosCertificados
    {
        get { return Set<ProcessamentoCertificados>(); }
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(CertificadosDbContext).Assembly);
    }
}
