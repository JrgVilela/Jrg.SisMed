using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Jrg.SisMed.Infra.Data.Migrations
{
    /// <inheritdoc />
    public partial class IncludeUserInProfessional : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "UserId",
                table: "Professionals",
                type: "int",
                nullable: false,
                defaultValue: 0,
                comment: "ID do usuário associado");

            migrationBuilder.CreateIndex(
                name: "IX_Professional_UserId",
                table: "Professionals",
                column: "UserId",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Professionals_Users",
                table: "Professionals",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Professionals_Users",
                table: "Professionals");

            migrationBuilder.DropIndex(
                name: "IX_Professional_UserId",
                table: "Professionals");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "Professionals");
        }
    }
}
