using GeradorCertificadosOnline.Dominio.Compartilhado;
using GeradorCertificadosOnline.Dominio.Modulos.Certificados;
using GeradorCertificadosOnline.Infraestrutura.Compartilhado.Orm;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

namespace GeradorCertificadosOnline.Infraestrutura.Modulos.Certificados;

/// <summary>
/// Implementa a persistência do agregado com EF Core e traduz conflitos do índice único
/// de processamento ativo em erro de negócio.
/// </summary>
public sealed class RepositorioProcessamentoCertificadosEmOrm(
    CertificadosDbContext db
) : IRepositorioProcessamentoCertificados
{
    /// <summary>
    /// Cria o lote e converte uma violação concorrente do índice único em conflito de negócio.
    /// </summary>
    public async Task<Guid> CriarLoteAsync(
        Guid cursoId,
        IReadOnlyList<Certificado> certificados,
        CancellationToken cancellationToken
    )
    {
        if (await ExisteEmAndamentoAsync(cursoId, cancellationToken))
            throw new ConflitoDePersistenciaException("Curso já possui um processamento em andamento.");

        var processamento = new ProcessamentoCertificados(cursoId, certificados);
        db.ProcessamentosCertificados.Add(processamento);

        try
        {
            await db.SaveChangesAsync(cancellationToken);
        }
        catch (DbUpdateException ex) when (ex.InnerException is SqlException { Number: 2601 or 2627 })
        {
            throw new ConflitoDePersistenciaException("Curso já possui um processamento em andamento.", ex);
        }

        return processamento.Id;
    }

    /// <summary>
    /// Reanexa o agregado desconectado para salvar o estado completo sem entidades rastreadas antigas.
    /// </summary>
    public async Task SalvarAsync(
        ProcessamentoCertificados processamento,
        CancellationToken cancellationToken
    )
    {
        db.ChangeTracker.Clear();
        db.ProcessamentosCertificados.Attach(processamento);

        db.Entry(processamento).State = EntityState.Modified;

        foreach (var certificado in processamento.Certificados)
            db.Entry(certificado).State = EntityState.Modified;

        await db.SaveChangesAsync(cancellationToken);

        db.ChangeTracker.Clear();
    }

    /// <summary>Seleciona o último lote pelo UUIDv7, incluindo seus certificados.</summary>
    public Task<ProcessamentoCertificados?> SelecionarPorCursoAsync(
        Guid cursoId,
        CancellationToken cancellationToken
    )
    {
        return db.ProcessamentosCertificados
            .AsNoTracking()
            .Include(p => p.Certificados)
            .Where(p => p.CursoId == cursoId)
            .OrderByDescending(p => p.Id)
            .FirstOrDefaultAsync(cancellationToken);
    }

    /// <summary>Carrega um lote com curso e certificados para o consumer.</summary>
    public Task<ProcessamentoCertificados?> SelecionarPorIdAsync(
        Guid id,
        CancellationToken cancellationToken
    )
    {
        return db.ProcessamentosCertificados
            .AsNoTracking()
            .Include(p => p.Certificados)
            .Include(p => p.Curso)
            .FirstOrDefaultAsync(p => p.Id == id, cancellationToken);
    }

    /// <summary>Verifica se há lote pendente ou em geração para o curso.</summary>
    public Task<bool> ExisteEmAndamentoAsync(Guid cursoId, CancellationToken cancellationToken)
    {
        return db.ProcessamentosCertificados.AnyAsync(
            p => p.CursoId == cursoId
                && (p.Status == StatusProcessamento.Pendente
                    || p.Status == StatusProcessamento.GerandoCertificados),
            cancellationToken);
    }
}
