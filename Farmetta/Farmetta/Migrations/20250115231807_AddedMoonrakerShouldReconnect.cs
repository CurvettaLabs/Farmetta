using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Farmetta.Migrations
{
    /// <inheritdoc />
    public partial class AddedMoonrakerShouldReconnect : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "ShouldReconnect",
                table: "MoonrakerInstances",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ShouldReconnect",
                table: "MoonrakerInstances");
        }
    }
}
