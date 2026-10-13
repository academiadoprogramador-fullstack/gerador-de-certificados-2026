using GeradorCertificadosOnline.Dominio.Modulos.Certificados;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GeradorCertificadosOnline.Infraestrutura.Compartilhado.Orm.Config;

public class ProcessamentoCertificadosConfiguration : IEntityTypeConfiguration<ProcessamentoCertificados>
{
    public void Configure(EntityTypeBuilder<ProcessamentoCertificados> builder)
    {
        builder.ToTable("ProcessamentosCertificados");
        builder.HasKey(p => p.Id);

        builder.HasIndex(p => p.CursoId)
            .IsUnique()
            .HasFilter("[Status] IN ('Pendente', 'GerandoCertificados')");

        builder.Property(p => p.Status)
            .HasConversion<string>()
            .HasMaxLength(30);

        builder.Property(p => p.CaminhoZip)
            .HasMaxLength(500);

        builder.HasOne(p => p.Curso)
            .WithMany(c => c.Processamentos)
            .HasForeignKey(p => p.CursoId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
