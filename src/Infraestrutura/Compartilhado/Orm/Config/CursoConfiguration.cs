using GeradorCertificadosOnline.Dominio.Modulos.Cursos;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GeradorCertificadosOnline.Infraestrutura.Compartilhado.Orm.Config;

public class CursoConfiguration : IEntityTypeConfiguration<Curso>
{
    public void Configure(EntityTypeBuilder<Curso> builder)
    {
        builder.Property(c => c.Nome)
            .HasMaxLength(Curso.TamanhoMaximoNome);
    }
}
