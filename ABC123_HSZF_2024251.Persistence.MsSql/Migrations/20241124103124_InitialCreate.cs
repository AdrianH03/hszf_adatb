﻿using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ABC123_HSZF_2024251.Persistence.MsSql.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "TaxiCars",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    LicensePlate = table.Column<string>(type: "TEXT", maxLength: 20, nullable: false),
                    Driver = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TaxiCars", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Fares",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    From = table.Column<string>(type: "TEXT", maxLength: 200, nullable: false),
                    To = table.Column<string>(type: "TEXT", maxLength: 200, nullable: false),
                    Distance = table.Column<double>(type: "REAL", nullable: false),
                    PaidAmount = table.Column<decimal>(type: "TEXT", nullable: false),
                    FareStartDate = table.Column<DateTime>(type: "TEXT", nullable: false),
                    TaxiCarId = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Fares", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Fares_TaxiCars_TaxiCarId",
                        column: x => x.TaxiCarId,
                        principalTable: "TaxiCars",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Fares_TaxiCarId",
                table: "Fares",
                column: "TaxiCarId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Fares");

            migrationBuilder.DropTable(
                name: "TaxiCars");
        }
    }
}
