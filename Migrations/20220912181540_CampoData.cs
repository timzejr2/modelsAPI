using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace apimodels.Migrations
{
    public partial class CampoData : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "DataPublicacao",
                table: "Maquetes",
                type: "datetimeoffset",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DataPublicacao",
                table: "Maquetes");
        }
    }
}
