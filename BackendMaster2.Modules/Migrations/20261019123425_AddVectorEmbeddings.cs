using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Pgvector;

#nullable disable

namespace BackendMaster2.Modules.Migrations
{
    /// <inheritdoc />
    public partial class AddVectorEmbeddings : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Solo añadimos la columna Embedding a tablas que YA existen
            migrationBuilder.AddColumn<Vector>(
                name: "Embedding",
                table: "Colors",
                type: "vector(1536)",
                nullable: true);

            migrationBuilder.AddColumn<Vector>(
                name: "Embedding",
                table: "Product",
                type: "vector(1536)",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(name: "Embedding", table: "Product");
            migrationBuilder.DropColumn(name: "Embedding", table: "Colors");
        }
    }
}
