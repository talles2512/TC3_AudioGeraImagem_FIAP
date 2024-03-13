using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AudioGeraImagemWorker.Infra.Migrations
{
    /// <inheritdoc />
    public partial class Inicial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Comandos",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UrlAudio = table.Column<string>(type: "VARCHAR(MAX)", nullable: true),
                    TextoProcessado = table.Column<string>(type: "VARCHAR(MAX)", nullable: true),
                    UrlImagem = table.Column<string>(type: "VARCHAR(MAX)", nullable: true),
                    InstanteCriacao = table.Column<DateTime>(type: "DATETIME2", nullable: false),
                    InstanteAtualizacao = table.Column<DateTime>(type: "DATETIME2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Comandos", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ProcessamentoComandos",
                columns: table => new
                {
                    ComandoId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Estado = table.Column<string>(type: "VARCHAR(20)", nullable: false),
                    InstanteCriacao = table.Column<DateTime>(type: "DATETIME2", nullable: false),
                    MensagemErro = table.Column<string>(type: "VARCHAR(256)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProcessamentoComandos", x => new { x.ComandoId, x.Id });
                    table.ForeignKey(
                        name: "FK_ProcessamentoComandos_Comandos_ComandoId",
                        column: x => x.ComandoId,
                        principalTable: "Comandos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ProcessamentoComandos");

            migrationBuilder.DropTable(
                name: "Comandos");
        }
    }
}
