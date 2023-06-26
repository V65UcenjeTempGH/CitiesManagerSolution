using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CitiesManager.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class IzmenaStruktureTabeleCity_DopunaPolja : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "CityHistory",
                table: "Cities",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "DateOfFoundation",
                table: "Cities",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Description",
                table: "Cities",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Population",
                table: "Cities",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "ZipCode",
                table: "Cities",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.UpdateData(
                table: "Cities",
                keyColumn: "CityID",
                keyValue: new Guid("459a76f0-f918-4965-b197-d8e0a3edeb27"),
                columns: new[] { "CityHistory", "CityName", "DateOfFoundation", "Description", "Population", "ZipCode" },
                values: new object[] { "...", "Београд", new DateTime(1500, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "...", 220000, "11000" });

            migrationBuilder.UpdateData(
                table: "Cities",
                keyColumn: "CityID",
                keyValue: new Guid("643b309f-d8c1-4b85-b48a-285cfdd69114"),
                columns: new[] { "CityHistory", "CityName", "DateOfFoundation", "Description", "Population", "ZipCode" },
                values: new object[] { "...", "Зрењанин", new DateTime(1500, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "...", 90000, "23000" });

            migrationBuilder.UpdateData(
                table: "Cities",
                keyColumn: "CityID",
                keyValue: new Guid("b6f78464-4403-4210-949f-236d28753fdd"),
                columns: new[] { "CityHistory", "CityName", "DateOfFoundation", "Description", "Population", "ZipCode" },
                values: new object[] { "...", "Нови Сад", new DateTime(1500, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "...", 45000, "21000" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CityHistory",
                table: "Cities");

            migrationBuilder.DropColumn(
                name: "DateOfFoundation",
                table: "Cities");

            migrationBuilder.DropColumn(
                name: "Description",
                table: "Cities");

            migrationBuilder.DropColumn(
                name: "Population",
                table: "Cities");

            migrationBuilder.DropColumn(
                name: "ZipCode",
                table: "Cities");

            migrationBuilder.UpdateData(
                table: "Cities",
                keyColumn: "CityID",
                keyValue: new Guid("459a76f0-f918-4965-b197-d8e0a3edeb27"),
                column: "CityName",
                value: "Beograd");

            migrationBuilder.UpdateData(
                table: "Cities",
                keyColumn: "CityID",
                keyValue: new Guid("643b309f-d8c1-4b85-b48a-285cfdd69114"),
                column: "CityName",
                value: "Zrenjanin");

            migrationBuilder.UpdateData(
                table: "Cities",
                keyColumn: "CityID",
                keyValue: new Guid("b6f78464-4403-4210-949f-236d28753fdd"),
                column: "CityName",
                value: "Novi Sad");
        }
    }
}
