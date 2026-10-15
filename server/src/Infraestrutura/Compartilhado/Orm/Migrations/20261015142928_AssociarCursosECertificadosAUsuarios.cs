using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GeradorCertificadosOnline.Infraestrutura.Compartilhado.Orm.Migrations
{
    /// <inheritdoc />
    public partial class AssociarCursosECertificadosAUsuarios : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "UsuarioId",
                table: "Cursos",
                type: "uniqueidentifier",
                nullable: false);

            migrationBuilder.AddColumn<Guid>(
                name: "UsuarioId",
                table: "Certificados",
                type: "uniqueidentifier",
                nullable: false);

            migrationBuilder.CreateIndex(
                name: "IX_Cursos_UsuarioId",
                table: "Cursos",
                column: "UsuarioId");

            migrationBuilder.CreateIndex(
                name: "IX_Certificados_UsuarioId",
                table: "Certificados",
                column: "UsuarioId");

            migrationBuilder.AddForeignKey(
                name: "FK_Certificados_AspNetUsers_UsuarioId",
                table: "Certificados",
                column: "UsuarioId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Cursos_AspNetUsers_UsuarioId",
                table: "Cursos",
                column: "UsuarioId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Certificados_AspNetUsers_UsuarioId",
                table: "Certificados");

            migrationBuilder.DropForeignKey(
                name: "FK_Cursos_AspNetUsers_UsuarioId",
                table: "Cursos");

            migrationBuilder.DropIndex(
                name: "IX_Cursos_UsuarioId",
                table: "Cursos");

            migrationBuilder.DropIndex(
                name: "IX_Certificados_UsuarioId",
                table: "Certificados");

            migrationBuilder.DropColumn(
                name: "UsuarioId",
                table: "Cursos");

            migrationBuilder.DropColumn(
                name: "UsuarioId",
                table: "Certificados");
        }
    }
}
