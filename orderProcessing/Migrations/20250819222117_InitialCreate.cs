using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace orderProcessing.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "PhoneReservations",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PlanType = table.Column<string>(type: "nvarchar(32)", maxLength: 32, nullable: false),
                    Storage = table.Column<string>(type: "nvarchar(16)", maxLength: 16, nullable: false),
                    Color = table.Column<string>(type: "nvarchar(32)", maxLength: 32, nullable: false),
                    HasTradeIn = table.Column<bool>(type: "bit", nullable: false),
                    ExtendedCoverage = table.Column<bool>(type: "bit", nullable: false),
                    PaymentOption = table.Column<string>(type: "nvarchar(32)", maxLength: 32, nullable: false),
                    AddSmartwatch = table.Column<bool>(type: "bit", nullable: false),
                    SmartwatchQty = table.Column<int>(type: "int", nullable: true),
                    AddBuds = table.Column<bool>(type: "bit", nullable: false),
                    BudsQty = table.Column<int>(type: "int", nullable: true),
                    AddCharger = table.Column<bool>(type: "bit", nullable: false),
                    ChargerQty = table.Column<int>(type: "int", nullable: true),
                    ClientId = table.Column<int>(type: "int", nullable: false),
                    FullName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Email = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
                    Phone = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PhoneReservations", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_PhoneReservations_ClientId",
                table: "PhoneReservations",
                column: "ClientId");

            migrationBuilder.CreateIndex(
                name: "IX_PhoneReservations_CreatedAt",
                table: "PhoneReservations",
                column: "CreatedAt");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PhoneReservations");
        }
    }
}
