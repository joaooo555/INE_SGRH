using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SGRH.Migrations
{
    /// <inheritdoc />
    public partial class AddFotoAndObservacoes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "foto_path",
                table: "colaborador",
                type: "nvarchar(255)",
                maxLength: 255,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "observacoes",
                table: "colaborador",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "foto_path",
                table: "colaborador");

            migrationBuilder.DropColumn(
                name: "observacoes",
                table: "colaborador");
        }
    }
}
