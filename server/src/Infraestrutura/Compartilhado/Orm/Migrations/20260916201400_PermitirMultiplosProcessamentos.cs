using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GeradorCertificadosOnline.Infraestrutura.Compartilhado.Orm.Migrations
{
    /// <inheritdoc />
    public partial class PermitirMultiplosProcessamentos : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_ProcessamentosCertificados_CursoId",
                table: "ProcessamentosCertificados");

            migrationBuilder.CreateIndex(
                name: "IX_ProcessamentosCertificados_CursoId",
                table: "ProcessamentosCertificados",
                column: "CursoId",
                unique: true,
                filter: "[Status] IN ('Pendente', 'GerandoCertificados', 'GerandoZip')");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_ProcessamentosCertificados_CursoId",
                table: "ProcessamentosCertificados");

            migrationBuilder.CreateIndex(
                name: "IX_ProcessamentosCertificados_CursoId",
                table: "ProcessamentosCertificados",
                column: "CursoId",
                unique: true);
        }
    }
}
