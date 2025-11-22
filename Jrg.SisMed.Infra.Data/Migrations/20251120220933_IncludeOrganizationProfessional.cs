using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Jrg.SisMed.Infra.Data.Migrations
{
    /// <inheritdoc />
    public partial class IncludeOrganizationProfessional : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "OrganizationProfessionals",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    OrganizationId = table.Column<int>(type: "int", nullable: false, comment: "ID da organização"),
                    ProfessionalId = table.Column<int>(type: "int", nullable: false, comment: "ID do profissional"),
                    State = table.Column<int>(type: "int", nullable: false, comment: "Estado da associação: 1=Active, 2=Inactive, 3=Blocked"),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()", comment: "Data de criação do registro"),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true, comment: "Data da última atualização do registro"),
                    CreatedById = table.Column<int>(type: "int", nullable: true, comment: "ID do profissional que criou a associação"),
                    UpdatedById = table.Column<int>(type: "int", nullable: true, comment: "ID do profissional que atualizou a associação")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OrganizationProfessionals", x => x.Id);
                    table.ForeignKey(
                        name: "FK_OrganizationProfessionals_CreatedBy",
                        column: x => x.CreatedById,
                        principalTable: "Professionals",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_OrganizationProfessionals_Organizations",
                        column: x => x.OrganizationId,
                        principalTable: "Organizations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_OrganizationProfessionals_Professionals",
                        column: x => x.ProfessionalId,
                        principalTable: "Professionals",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_OrganizationProfessionals_UpdatedBy",
                        column: x => x.UpdatedById,
                        principalTable: "Professionals",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_OrganizationProfessionals_CreatedAt",
                table: "OrganizationProfessionals",
                column: "CreatedAt");

            migrationBuilder.CreateIndex(
                name: "IX_OrganizationProfessionals_CreatedById",
                table: "OrganizationProfessionals",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_OrganizationProfessionals_OrganizationId",
                table: "OrganizationProfessionals",
                column: "OrganizationId");

            migrationBuilder.CreateIndex(
                name: "IX_OrganizationProfessionals_OrgId_ProfId",
                table: "OrganizationProfessionals",
                columns: new[] { "OrganizationId", "ProfessionalId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_OrganizationProfessionals_ProfessionalId",
                table: "OrganizationProfessionals",
                column: "ProfessionalId");

            migrationBuilder.CreateIndex(
                name: "IX_OrganizationProfessionals_State",
                table: "OrganizationProfessionals",
                column: "State");

            migrationBuilder.CreateIndex(
                name: "IX_OrganizationProfessionals_UpdatedById",
                table: "OrganizationProfessionals",
                column: "UpdatedById");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "OrganizationProfessionals");
        }
    }
}
