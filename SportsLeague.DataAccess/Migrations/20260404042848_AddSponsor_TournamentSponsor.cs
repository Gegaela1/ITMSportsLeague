using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SportsLeague.DataAccess.Migrations
{
    public partial class AddSponsor_TournamentSponsor : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // ✅ Crear tabla Sponsors correctamente
            migrationBuilder.CreateTable(
                name: "Sponsors",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    ContactEmail = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Sponsors", x => x.Id);
                });

            // ✅ (Opcional pero recomendado) Crear índice
            migrationBuilder.CreateIndex(
                name: "IX_Sponsors_Name",
                table: "Sponsors",
                column: "Name");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // ✅ Eliminar tabla si se hace rollback
            migrationBuilder.DropTable(
                name: "Sponsors");
        }
    }
}

