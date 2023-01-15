using Microsoft.EntityFrameworkCore.Migrations;

namespace NoteAPI.Migrations
{
    public partial class MiddlewareAdd : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "NotePrority",
                table: "Notes");

            migrationBuilder.RenameColumn(
                name: "Date",
                table: "Notes",
                newName: "CreatedDate");

            migrationBuilder.AddColumn<bool>(
                name: "IsImportant",
                table: "Notes",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsImportant",
                table: "Notes");

            migrationBuilder.RenameColumn(
                name: "CreatedDate",
                table: "Notes",
                newName: "Date");

            migrationBuilder.AddColumn<int>(
                name: "NotePrority",
                table: "Notes",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }
    }
}
