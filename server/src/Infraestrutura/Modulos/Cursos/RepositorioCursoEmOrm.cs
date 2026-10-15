using GeradorCertificadosOnline.Dominio.Modulos.Cursos;
using GeradorCertificadosOnline.Infraestrutura.Compartilhado.Orm;
using Microsoft.EntityFrameworkCore;

namespace GeradorCertificadosOnline.Infraestrutura.Modulos.Cursos;

public sealed class RepositorioCursoEmOrm(CertificadosDbContext db) : IRepositorioCurso
{
    public async Task CadastrarAsync(Curso curso, CancellationToken cancellationToken)
    {
        db.Cursos.Add(curso);
        await db.SaveChangesAsync(cancellationToken);
    }

    public Task<Curso?> SelecionarPorIdAsync(Guid id, CancellationToken cancellationToken)
    {
        return db.Cursos.AsNoTracking().FirstOrDefaultAsync(c => c.Id == id, cancellationToken);
    }

    public async Task<IReadOnlyList<Curso>> SelecionarTodosAsync(CancellationToken cancellationToken)
    {
        return await db.Cursos
            .AsNoTracking()
            .OrderBy(c => c.Nome)
            .ThenBy(c => c.Id)
            .ToListAsync(cancellationToken);
    }

    public Task<bool> ExisteAsync(Guid id, CancellationToken cancellationToken)
    {
        return db.Cursos.AnyAsync(c => c.Id == id, cancellationToken);
    }
}
