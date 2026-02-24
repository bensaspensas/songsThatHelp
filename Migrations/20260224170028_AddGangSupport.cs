using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SongsThatHelp.Migrations
{
    /// <inheritdoc />
    public partial class AddGangSupport : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "GangName",
                table: "Users",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "GangName",
                table: "Songs",
                type: "text",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Gangs",
                columns: table => new
                {
                    Name = table.Column<string>(type: "text", nullable: false),
                    PasswordHash = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Gangs", x => x.Name);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Gangs");

            migrationBuilder.DropColumn(
                name: "GangName",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "GangName",
                table: "Songs");
        }
    }
}
