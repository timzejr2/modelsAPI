using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace apimodels.Migrations
{
    public partial class TabelaMaquetes : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Maquetes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    NomeMaquete = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DescricaoMaquete = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ImgMaquete = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ArquivoMaquete = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    MaqueteAtiva = table.Column<bool>(type: "bit", nullable: false),
                    IdUsuario = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Maquetes", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Maquetes");
        }
    }
}
