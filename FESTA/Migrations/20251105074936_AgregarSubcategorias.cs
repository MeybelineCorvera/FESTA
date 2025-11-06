using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FESTA.Migrations
{
    /// <inheritdoc />
    public partial class AgregarSubcategorias : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "CategoriaPadreId",
                table: "Categorias",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Categorias_CategoriaPadreId",
                table: "Categorias",
                column: "CategoriaPadreId");

            migrationBuilder.AddForeignKey(
                name: "FK_Categorias_Categorias_CategoriaPadreId",
                table: "Categorias",
                column: "CategoriaPadreId",
                principalTable: "Categorias",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Categorias_Categorias_CategoriaPadreId",
                table: "Categorias");

            migrationBuilder.DropIndex(
                name: "IX_Categorias_CategoriaPadreId",
                table: "Categorias");

            migrationBuilder.DropColumn(
                name: "CategoriaPadreId",
                table: "Categorias");
        }
    }
}
