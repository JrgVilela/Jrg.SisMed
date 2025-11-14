using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Jrg.SisMed.Infra.Data.Migrations
{
    /// <inheritdoc />
    public partial class PhonesNumberAjuste : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Number",
                table: "Phones",
                type: "nvarchar(10)",
                maxLength: 10,
                nullable: false,
                comment: "Número do telefone (8 ou 9 dígitos)",
                oldClrType: typeof(string),
                oldType: "nvarchar(9)",
                oldMaxLength: 9,
                oldComment: "Número do telefone (8 ou 9 dígitos)");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Number",
                table: "Phones",
                type: "nvarchar(9)",
                maxLength: 9,
                nullable: false,
                comment: "Número do telefone (8 ou 9 dígitos)",
                oldClrType: typeof(string),
                oldType: "nvarchar(10)",
                oldMaxLength: 10,
                oldComment: "Número do telefone (8 ou 9 dígitos)");
        }
    }
}
