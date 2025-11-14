using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Jrg.SisMed.Infra.Data.Migrations
{
    /// <inheritdoc />
    public partial class ProfessionalCpfAjuste : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Cpf",
                table: "Professionals",
                type: "nvarchar(14)",
                maxLength: 14,
                nullable: false,
                comment: "CPF do profissional (apenas números)",
                oldClrType: typeof(string),
                oldType: "nvarchar(11)",
                oldMaxLength: 11,
                oldComment: "CPF do profissional (apenas números)");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Cpf",
                table: "Professionals",
                type: "nvarchar(11)",
                maxLength: 11,
                nullable: false,
                comment: "CPF do profissional (apenas números)",
                oldClrType: typeof(string),
                oldType: "nvarchar(14)",
                oldMaxLength: 14,
                oldComment: "CPF do profissional (apenas números)");
        }
    }
}
