using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ABC123_HSZF_2024251.Persistence.MsSql.Migrations
{
    /// <inheritdoc />
    public partial class AddLazyLoading : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("PRAGMA foreign_keys = OFF;", suppressTransaction: true);
            migrationBuilder.DropForeignKey(
                name: "FK_Fares_TaxiCars_TaxiCarId",
                table: "Fares");

            migrationBuilder.DropPrimaryKey(
                name: "PK_TaxiCars",
                table: "TaxiCars");

            migrationBuilder.RenameTable(
                name: "TaxiCars",
                newName: "TaxiCar");

            migrationBuilder.AddPrimaryKey(
                name: "PK_TaxiCar",
                table: "TaxiCar",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Fares_TaxiCar_TaxiCarId",
                table: "Fares",
                column: "TaxiCarId",
                principalTable: "TaxiCar",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
            migrationBuilder.Sql("PRAGMA foreign_keys = ON;", suppressTransaction: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Fares_TaxiCar_TaxiCarId",
                table: "Fares");

            migrationBuilder.DropPrimaryKey(
                name: "PK_TaxiCar",
                table: "TaxiCar");

            migrationBuilder.RenameTable(
                name: "TaxiCar",
                newName: "TaxiCars");

            migrationBuilder.AddPrimaryKey(
                name: "PK_TaxiCars",
                table: "TaxiCars",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Fares_TaxiCars_TaxiCarId",
                table: "Fares",
                column: "TaxiCarId",
                principalTable: "TaxiCars",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
