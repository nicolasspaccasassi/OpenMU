using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MUnique.OpenMU.Persistence.EntityFramework.Migrations
{
    /// <inheritdoc />
    public partial class AddHardwareBan : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "LastKnownHardwareId",
                schema: "data",
                table: "Account",
                type: "text",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "BannedHardwareId",
                schema: "data",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    HardwareIdHash = table.Column<string>(type: "text", nullable: false),
                    BannedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Reason = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BannedHardwareId", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_BannedHardwareId_HardwareIdHash",
                schema: "data",
                table: "BannedHardwareId",
                column: "HardwareIdHash");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "BannedHardwareId",
                schema: "data");

            migrationBuilder.DropColumn(
                name: "LastKnownHardwareId",
                schema: "data",
                table: "Account");
        }
    }
}
