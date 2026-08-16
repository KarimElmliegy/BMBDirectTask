using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BMBAssessment.Infrastructure.Migrations
{
    public partial class AddProductVariantInventory : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Quantity",
                table: "ProductVariants",
                type: "int",
                nullable: false,
                defaultValue: 5);

            migrationBuilder.AddColumn<byte[]>(
                name: "Version",
                table: "ProductVariants",
                type: "rowversion",
                rowVersion: true,
                nullable: false,
                defaultValue: new byte[0]);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Quantity",
                table: "ProductVariants");

            migrationBuilder.DropColumn(
                name: "Version",
                table: "ProductVariants");
        }
    }
}
