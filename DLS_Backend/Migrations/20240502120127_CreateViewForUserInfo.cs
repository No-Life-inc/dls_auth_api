using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DLS_Backend.Migrations
{
    /// <inheritdoc />
    public partial class CreateViewForUserInfo : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(
                @"CREATE VIEW LatestUserInfosView AS
                WITH LatestUserInfos AS (
                    SELECT UserId, MAX(created_at) AS MaxCreatedAt
                    FROM UserInfo
                    GROUP BY UserId
                )
                SELECT UI.*
                FROM UserInfo UI
                INNER JOIN LatestUserInfos LUI ON UI.UserId = LUI.UserId AND UI.created_at = LUI.MaxCreatedAt");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("DROP VIEW LatestUserInfosView");
        }
    }
}
