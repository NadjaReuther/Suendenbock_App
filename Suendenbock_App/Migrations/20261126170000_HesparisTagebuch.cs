using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Suendenbock_App.Migrations
{
    /// <inheritdoc />
    public partial class HesparisTagebuch : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Tagebuch: "überarbeitet" bei Ausgaben, Abhak-Zeitpunkt bei Kleinanzeigen.
            migrationBuilder.AddColumn<DateTime>(
                name: "BearbeitetAm",
                table: "ProtokollEintraege",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "ErledigtAm",
                table: "Aufgaben",
                type: "datetime2",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "BearbeitetAm",
                table: "ProtokollEintraege");

            migrationBuilder.DropColumn(
                name: "ErledigtAm",
                table: "Aufgaben");
        }
    }
}
