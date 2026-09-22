using GeradorCertificadosOnline.Dominio.Modulos.Certificados;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GeradorCertificadosOnline.Infraestrutura.Compartilhado.Orm.Config;

public class CertificadoConfiguration : IEntityTypeConfiguration<Certificado>
{
    public void Configure(EntityTypeBuilder<Certificado> builder)
    {
        builder.Property(c => c.NomeAluno)
            .HasMaxLength(Certificado.TamanhoMaximoNome);

        builder.Property(c => c.StatusGeracao)
            .HasConversion<string>()
            .HasMaxLength(30);

        builder.Property(c => c.CaminhoArquivo)
            .HasMaxLength(500);

        builder.HasOne(c => c.Processamento)
            .WithMany(p => p.Certificados)
            .HasForeignKey(c => c.ProcessamentoId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(c => new { c.ProcessamentoId, c.StatusGeracao });
    }
}
