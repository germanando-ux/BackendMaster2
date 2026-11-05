using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BackendMaster2.Modules.Migrations
{
    /// <inheritdoc />
    public partial class RefactorProductFeatureKey : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_ProductFeatures",
                table: "ProductFeatures");

            migrationBuilder.AlterColumn<string>(
                name: "Value",
                table: "ProductFeatures",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AddColumn<Guid>(
                name: "Id",
                table: "ProductFeatures",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<Guid>(
                name: "FeatureOptionId",
                table: "ProductFeatures",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_ProductFeatures",
                table: "ProductFeatures",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_ProductFeatures_FeatureOptionId",
                table: "ProductFeatures",
                column: "FeatureOptionId");

            migrationBuilder.CreateIndex(
                name: "IX_ProductFeatures_ProductId",
                table: "ProductFeatures",
                column: "ProductId");

            migrationBuilder.AddForeignKey(
                name: "FK_ProductFeatures_FeatureOptions_FeatureOptionId",
                table: "ProductFeatures",
                column: "FeatureOptionId",
                principalTable: "FeatureOptions",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ProductFeatures_FeatureOptions_FeatureOptionId",
                table: "ProductFeatures");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ProductFeatures",
                table: "ProductFeatures");

            migrationBuilder.DropIndex(
                name: "IX_ProductFeatures_FeatureOptionId",
                table: "ProductFeatures");

            migrationBuilder.DropIndex(
                name: "IX_ProductFeatures_ProductId",
                table: "ProductFeatures");

            migrationBuilder.DropColumn(
                name: "Id",
                table: "ProductFeatures");

            migrationBuilder.DropColumn(
                name: "FeatureOptionId",
                table: "ProductFeatures");

            migrationBuilder.AlterColumn<string>(
                name: "Value",
                table: "ProductFeatures",
                type: "text",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_ProductFeatures",
                table: "ProductFeatures",
                columns: new[] { "ProductId", "FeatureId" });
        }
    }
}
