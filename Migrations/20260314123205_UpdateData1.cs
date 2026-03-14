using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace reminder_schedule_backend.Migrations
{
    /// <inheritdoc />
    public partial class UpdateData1 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "name",
                table: "Teachers",
                newName: "Name");

            migrationBuilder.RenameColumn(
                name: "id",
                table: "Teachers",
                newName: "Id");

            migrationBuilder.RenameColumn(
                name: "password",
                table: "Admins",
                newName: "Password");

            migrationBuilder.RenameColumn(
                name: "email",
                table: "Admins",
                newName: "Email");

            migrationBuilder.RenameColumn(
                name: "id",
                table: "Admins",
                newName: "Id");

            migrationBuilder.RenameColumn(
                name: "nama",
                table: "Admins",
                newName: "Name");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Name",
                table: "Teachers",
                newName: "name");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "Teachers",
                newName: "id");

            migrationBuilder.RenameColumn(
                name: "Password",
                table: "Admins",
                newName: "password");

            migrationBuilder.RenameColumn(
                name: "Email",
                table: "Admins",
                newName: "email");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "Admins",
                newName: "id");

            migrationBuilder.RenameColumn(
                name: "Name",
                table: "Admins",
                newName: "nama");
        }
    }
}
