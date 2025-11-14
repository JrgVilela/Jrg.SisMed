using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Jrg.SisMed.Infra.Data.Migrations
{
    /// <inheritdoc />
    public partial class OrganizationCnpjAjuste : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Cnpj",
                table: "Organizations",
                type: "nvarchar(18)",
                maxLength: 18,
                nullable: false,
                comment: "Cnpj da organização (apenas números)",
                oldClrType: typeof(string),
                oldType: "nvarchar(14)",
                oldMaxLength: 14,
                oldComment: "Cnpj da organização (apenas números)");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Cnpj",
                table: "Organizations",
                type: "nvarchar(14)",
                maxLength: 14,
                nullable: false,
                comment: "Cnpj da organização (apenas números)",
                oldClrType: typeof(string),
                oldType: "nvarchar(18)",
                oldMaxLength: 18,
                oldComment: "Cnpj da organização (apenas números)");
        }
    }
}
