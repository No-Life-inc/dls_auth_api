using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DLS_Backend.Migrations
{
    /// <inheritdoc />
    public partial class AddUserTombstone : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "UserTombstones",
                columns: table => new
                {
                    UserId = table.Column<int>(type: "int", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    DeletionDate = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserTombstones", x => x.UserId);
                    table.ForeignKey(
                        name: "FK_UserTombstones_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "LatestUserInfosView");

            migrationBuilder.DropTable(
                name: "UserTombstones");
        }
    }
}
