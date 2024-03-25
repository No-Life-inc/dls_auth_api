using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DLS_Backend.Migrations
{
    /// <inheritdoc />
    public partial class CreateUserTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    guid = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    first_name = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    last_name = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    password = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    created_at = table.Column<DateTime>(type: "datetime2", nullable: true, defaultValueSql: "(getdate())"),
                    email = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Users__3214EC0732BBF347", x => x.id);
                });
            
            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "guid", "first_name", "last_name", "password", "email" },
                values: new object[] { Guid.NewGuid(), "Zack", "Ottesen", "$2a$11$ehQatcIZKN5sEL9Z9e10belJSKQFD7ghaQmJER4wBKG7lEJQR.rvm", "zo@pyra.dk" });

            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "guid", "first_name", "last_name", "password", "email" },
                values: new object[] { Guid.NewGuid(), "John", "Doe", "$2a$11$EiQBIUoyFU6DSkanj1F/DOeS8fd3Mpwr8S3TB2iafVFoby6Innoxe", "jane.doe@example.com" }); 
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Users");
        }
    }
}
