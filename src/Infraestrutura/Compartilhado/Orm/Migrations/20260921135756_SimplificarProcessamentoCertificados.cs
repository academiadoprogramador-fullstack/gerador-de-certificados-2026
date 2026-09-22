using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GeradorCertificadosOnline.Infraestrutura.Compartilhado.Orm.Migrations
{
    /// <inheritdoc />
    public partial class SimplificarProcessamentoCertificados : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_ProcessamentosCertificados_CursoId",
                table: "ProcessamentosCertificados");

            migrationBuilder.DropColumn(
                name: "Falhas",
                table: "ProcessamentosCertificados");

            migrationBuilder.DropColumn(
                name: "Gerados",
                table: "ProcessamentosCertificados");

            migrationBuilder.DropColumn(
                name: "TotalCertificados",
                table: "ProcessamentosCertificados");

            migrationBuilder.CreateIndex(
                name: "IX_ProcessamentosCertificados_CursoId",
                table: "ProcessamentosCertificados",
                column: "CursoId",
                unique: true,
                filter: "[Status] IN ('Pendente', 'GerandoCertificados')");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_ProcessamentosCertificados_CursoId",
                table: "ProcessamentosCertificados");

            migrationBuilder.AddColumn<int>(
                name: "Falhas",
                table: "ProcessamentosCertificados",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "Gerados",
                table: "ProcessamentosCertificados",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "TotalCertificados",
                table: "ProcessamentosCertificados",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_ProcessamentosCertificados_CursoId",
                table: "ProcessamentosCertificados",
                column: "CursoId",
                unique: true,
                filter: "[Status] IN ('Pendente', 'GerandoCertificados', 'GerandoZip')");
        }
    }
}
