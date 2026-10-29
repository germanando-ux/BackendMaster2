using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BackendMaster2.Modules.Migrations
{
    /// <inheritdoc />
    public partial class AddProductSourcesAndMarketplaces : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "Price",
                table: "ProductSizes",
                type: "numeric",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "ProductDataSources",
                columns: table => new
                {
                    ProductId = table.Column<Guid>(type: "uuid", nullable: false),
                    DataSourceId = table.Column<Guid>(type: "uuid", nullable: false),
                    ExternalCode = table.Column<string>(type: "text", nullable: false),
                    LastSyncedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProductDataSources", x => new { x.ProductId, x.DataSourceId });
                    table.ForeignKey(
                        name: "FK_ProductDataSources_DataSources_DataSourceId",
                        column: x => x.DataSourceId,
                        principalTable: "DataSources",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ProductDataSources_Product_ProductId",
                        column: x => x.ProductId,
                        principalTable: "Product",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ProductMarketplaces",
                columns: table => new
                {
                    ProductId = table.Column<Guid>(type: "uuid", nullable: false),
                    MarketplaceId = table.Column<Guid>(type: "uuid", nullable: false),
                    ExternalCode = table.Column<string>(type: "text", nullable: false),
                    LastSyncedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProductMarketplaces", x => new { x.ProductId, x.MarketplaceId });
                    table.ForeignKey(
                        name: "FK_ProductMarketplaces_Marketplaces_MarketplaceId",
                        column: x => x.MarketplaceId,
                        principalTable: "Marketplaces",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ProductMarketplaces_Product_ProductId",
                        column: x => x.ProductId,
                        principalTable: "Product",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ProductDataSources_DataSourceId",
                table: "ProductDataSources",
                column: "DataSourceId");

            migrationBuilder.CreateIndex(
                name: "IX_ProductMarketplaces_MarketplaceId",
                table: "ProductMarketplaces",
                column: "MarketplaceId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ProductDataSources");

            migrationBuilder.DropTable(
                name: "ProductMarketplaces");

            migrationBuilder.DropColumn(
                name: "Price",
                table: "ProductSizes");
        }
    }
}
