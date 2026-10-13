using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GeradorCertificadosOnline.Infraestrutura.Compartilhado.Orm.Migrations
{
    /// <inheritdoc />
    public partial class ProcessamentoCertificadosRelacionaCertificados : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Certificados_Cursos_CursoId",
                table: "Certificados");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ProcessamentosCertificados",
                table: "ProcessamentosCertificados");

            migrationBuilder.RenameColumn(
                name: "CursoId",
                table: "Certificados",
                newName: "ProcessamentoId");

            migrationBuilder.RenameIndex(
                name: "IX_Certificados_CursoId_StatusGeracao",
                table: "Certificados",
                newName: "IX_Certificados_ProcessamentoId_StatusGeracao");

            migrationBuilder.AddColumn<Guid>(
                name: "Id",
                table: "ProcessamentosCertificados",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.Sql(
                "UPDATE ProcessamentosCertificados SET Id = NEWID() WHERE Id IS NULL;");

            migrationBuilder.AlterColumn<Guid>(
                name: "Id",
                table: "ProcessamentosCertificados",
                type: "uniqueidentifier",
                nullable: false,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true);

            migrationBuilder.Sql(
                "UPDATE c " +
                "SET ProcessamentoId = p.Id " +
                "FROM Certificados c " +
                "INNER JOIN ProcessamentosCertificados p ON p.CursoId = c.ProcessamentoId;");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ProcessamentosCertificados",
                table: "ProcessamentosCertificados",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_ProcessamentosCertificados_CursoId",
                table: "ProcessamentosCertificados",
                column: "CursoId",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Certificados_ProcessamentosCertificados_ProcessamentoId",
                table: "Certificados",
                column: "ProcessamentoId",
                principalTable: "ProcessamentosCertificados",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Certificados_ProcessamentosCertificados_ProcessamentoId",
                table: "Certificados");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ProcessamentosCertificados",
                table: "ProcessamentosCertificados");

            migrationBuilder.DropIndex(
                name: "IX_ProcessamentosCertificados_CursoId",
                table: "ProcessamentosCertificados");

            migrationBuilder.Sql(
                "UPDATE c " +
                "SET ProcessamentoId = p.CursoId " +
                "FROM Certificados c " +
                "INNER JOIN ProcessamentosCertificados p ON p.Id = c.ProcessamentoId;");

            migrationBuilder.DropColumn(
                name: "Id",
                table: "ProcessamentosCertificados");

            migrationBuilder.RenameColumn(
                name: "ProcessamentoId",
                table: "Certificados",
                newName: "CursoId");

            migrationBuilder.RenameIndex(
                name: "IX_Certificados_ProcessamentoId_StatusGeracao",
                table: "Certificados",
                newName: "IX_Certificados_CursoId_StatusGeracao");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ProcessamentosCertificados",
                table: "ProcessamentosCertificados",
                column: "CursoId");

            migrationBuilder.AddForeignKey(
                name: "FK_Certificados_Cursos_CursoId",
                table: "Certificados",
                column: "CursoId",
                principalTable: "Cursos",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
