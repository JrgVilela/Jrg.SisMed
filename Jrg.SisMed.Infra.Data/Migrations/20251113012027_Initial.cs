using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Jrg.SisMed.Infra.Data.Migrations
{
    /// <inheritdoc />
    public partial class Initial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Organizations",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    NameFantasia = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false, comment: "Nome fantasia da organização (nome comercial)"),
                    RazaoSocial = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false, comment: "Razão social da organização (nome jurídico)"),
                    Cnpj = table.Column<string>(type: "nvarchar(14)", maxLength: 14, nullable: false, comment: "Cnpj da organização (apenas números)"),
                    State = table.Column<int>(type: "int", nullable: false, comment: "Estado da organização: 1=Active, 2=Inactive, 3=Suspended"),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()", comment: "Data de criação do registro"),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true, comment: "Data da última atualização do registro")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Organizations", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Professionals",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false, comment: "Nome completo do Professional"),
                    Cpf = table.Column<string>(type: "nvarchar(11)", maxLength: 11, nullable: false, comment: "CPF do profissional (apenas números)"),
                    Rg = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true, comment: "RG do profissional"),
                    BirthDate = table.Column<DateTime>(type: "datetime2", nullable: true, comment: "Data de nascimento"),
                    Gender = table.Column<int>(type: "int", nullable: false, comment: "Gênero: 0=None, 1=Male, 2=Female, 3=Other"),
                    State = table.Column<int>(type: "int", nullable: false, comment: "Estado do profissional: 1=Active, 2=Inactive"),
                    Email = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false, comment: "E-mail do profissional"),
                    PasswordHash = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false, comment: "Hash da senha (PBKDF2)"),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()", comment: "Data de criação do registro"),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true, comment: "Data da última atualização do registro")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Professionals", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Addresses",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Street = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false, comment: "Nome da rua/logradouro"),
                    Number = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false, comment: "Número do endereço"),
                    Complement = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true, comment: "Complemento do endereço"),
                    Neighborhood = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ZipCode = table.Column<string>(type: "nvarchar(8)", maxLength: 8, nullable: false, comment: "CEP (apenas números)"),
                    City = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false, comment: "Cidade"),
                    State = table.Column<string>(type: "nvarchar(2)", maxLength: 2, nullable: false, comment: "Estado/UF (sigla)"),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()", comment: "Data de criação do registro"),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true, comment: "Data da última atualização do registro"),
                    CreatedById = table.Column<int>(type: "int", nullable: true, comment: "ID da pessoa que criou o registro"),
                    UpdatedById = table.Column<int>(type: "int", nullable: true, comment: "ID da pessoa que atualizou o registro")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Addresses", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Addresses_Professionals_CreatedById",
                        column: x => x.CreatedById,
                        principalTable: "Professionals",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Addresses_Professionals_UpdatedById",
                        column: x => x.UpdatedById,
                        principalTable: "Professionals",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Nutritionists",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false),
                    Crn = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false, comment: "Número do registro no Conselho Regional de Nutricionistas (CRN)")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Nutritionists", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Nutritionists_Professionals_Id",
                        column: x => x.Id,
                        principalTable: "Professionals",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Phones",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Ddi = table.Column<string>(type: "nvarchar(3)", maxLength: 3, nullable: false, comment: "Código DDI (código internacional do país)"),
                    Ddd = table.Column<string>(type: "nvarchar(2)", maxLength: 2, nullable: false, comment: "Código DDD (código de área)"),
                    Number = table.Column<string>(type: "nvarchar(9)", maxLength: 9, nullable: false, comment: "Número do telefone (8 ou 9 dígitos)"),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()", comment: "Data de criação do registro"),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true, comment: "Data da última atualização do registro"),
                    CreatedById = table.Column<int>(type: "int", nullable: true, comment: "ID da pessoa que criou o registro"),
                    UpdatedById = table.Column<int>(type: "int", nullable: true, comment: "ID da pessoa que atualizou o registro")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Phones", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Phones_Professionals_CreatedById",
                        column: x => x.CreatedById,
                        principalTable: "Professionals",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Phones_Professionals_UpdatedById",
                        column: x => x.UpdatedById,
                        principalTable: "Professionals",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Psychologists",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false),
                    Crp = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false, comment: "Número do registro no Conselho Regional de Psicologia (CRP)")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Psychologists", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Psychologists_Professionals_Id",
                        column: x => x.Id,
                        principalTable: "Professionals",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ProfessionalAddresses",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ProfessionalId = table.Column<int>(type: "int", nullable: false, comment: "ID do profissional"),
                    AddressId = table.Column<int>(type: "int", nullable: false, comment: "ID do endereço"),
                    IsPrincipal = table.Column<bool>(type: "bit", nullable: false, defaultValue: false, comment: "Indica se é o endereço principal"),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()", comment: "Data de criação do registro"),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true, comment: "Data da última atualização do registro"),
                    CreatedById = table.Column<int>(type: "int", nullable: true, comment: "ID do profissional que criou o registro"),
                    UpdatedById = table.Column<int>(type: "int", nullable: true, comment: "ID do profissional que atualizou o registro")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProfessionalAddresses", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ProfessionalAddresses_Addresses_AddressId",
                        column: x => x.AddressId,
                        principalTable: "Addresses",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ProfessionalAddresses_Professionals_CreatedById",
                        column: x => x.CreatedById,
                        principalTable: "Professionals",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ProfessionalAddresses_Professionals_ProfessionalId",
                        column: x => x.ProfessionalId,
                        principalTable: "Professionals",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ProfessionalAddresses_Professionals_UpdatedById",
                        column: x => x.UpdatedById,
                        principalTable: "Professionals",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "OrganizationPhones",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    OrganizationId = table.Column<int>(type: "int", nullable: false, comment: "ID da organização"),
                    PhoneId = table.Column<int>(type: "int", nullable: false, comment: "ID do telefone"),
                    IsPrincipal = table.Column<bool>(type: "bit", nullable: false, defaultValue: false, comment: "Indica se é o telefone principal"),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()", comment: "Data de criação do registro"),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true, comment: "Data da última atualização do registro"),
                    CreatedById = table.Column<int>(type: "int", nullable: true, comment: "ID da pessoa que criou o registro"),
                    UpdatedById = table.Column<int>(type: "int", nullable: true, comment: "ID da pessoa que atualizou o registro")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OrganizationPhones", x => x.Id);
                    table.ForeignKey(
                        name: "FK_OrganizationPhones_Organizations_OrganizationId",
                        column: x => x.OrganizationId,
                        principalTable: "Organizations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_OrganizationPhones_Phones_PhoneId",
                        column: x => x.PhoneId,
                        principalTable: "Phones",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_OrganizationPhones_Professionals_CreatedById",
                        column: x => x.CreatedById,
                        principalTable: "Professionals",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_OrganizationPhones_Professionals_UpdatedById",
                        column: x => x.UpdatedById,
                        principalTable: "Professionals",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ProfessionalPhones",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ProfessionalId = table.Column<int>(type: "int", nullable: false, comment: "ID o profissional"),
                    PhoneId = table.Column<int>(type: "int", nullable: false, comment: "ID do telefone"),
                    IsPrincipal = table.Column<bool>(type: "bit", nullable: false, defaultValue: false, comment: "Indica se é o telefone principal"),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()", comment: "Data de criação do registro"),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true, comment: "Data da última atualização do registro"),
                    CreatedById = table.Column<int>(type: "int", nullable: true, comment: "ID do profissional que criou o registro"),
                    UpdatedById = table.Column<int>(type: "int", nullable: true, comment: "ID do profissional que atualizou o registro")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProfessionalPhones", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ProfessionalPhones_Phones_PhoneId",
                        column: x => x.PhoneId,
                        principalTable: "Phones",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ProfessionalPhones_Professionals_CreatedById",
                        column: x => x.CreatedById,
                        principalTable: "Professionals",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ProfessionalPhones_Professionals_ProfessionalId",
                        column: x => x.ProfessionalId,
                        principalTable: "Professionals",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ProfessionalPhones_Professionals_UpdatedById",
                        column: x => x.UpdatedById,
                        principalTable: "Professionals",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Addresses_City_State",
                table: "Addresses",
                columns: new[] { "City", "State" });

            migrationBuilder.CreateIndex(
                name: "IX_Addresses_CreatedAt",
                table: "Addresses",
                column: "CreatedAt");

            migrationBuilder.CreateIndex(
                name: "IX_Addresses_CreatedById",
                table: "Addresses",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_Addresses_UpdatedById",
                table: "Addresses",
                column: "UpdatedById");

            migrationBuilder.CreateIndex(
                name: "IX_Addresses_ZipCode",
                table: "Addresses",
                column: "ZipCode");

            migrationBuilder.CreateIndex(
                name: "IX_Nutritionists_Crn",
                table: "Nutritionists",
                column: "Crn",
                unique: true,
                filter: "[Crn] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_OrganizationPhones_CreatedById",
                table: "OrganizationPhones",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_OrganizationPhones_IsPrincipal",
                table: "OrganizationPhones",
                column: "IsPrincipal");

            migrationBuilder.CreateIndex(
                name: "IX_OrganizationPhones_OrganizationId",
                table: "OrganizationPhones",
                column: "OrganizationId");

            migrationBuilder.CreateIndex(
                name: "IX_OrganizationPhones_OrganizationId_PhoneId",
                table: "OrganizationPhones",
                columns: new[] { "OrganizationId", "PhoneId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_OrganizationPhones_PhoneId",
                table: "OrganizationPhones",
                column: "PhoneId");

            migrationBuilder.CreateIndex(
                name: "IX_OrganizationPhones_UpdatedById",
                table: "OrganizationPhones",
                column: "UpdatedById");

            migrationBuilder.CreateIndex(
                name: "IX_Organizations_CNPJ",
                table: "Organizations",
                column: "Cnpj",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Organizations_CreatedAt",
                table: "Organizations",
                column: "CreatedAt");

            migrationBuilder.CreateIndex(
                name: "IX_Organizations_State",
                table: "Organizations",
                column: "State");

            migrationBuilder.CreateIndex(
                name: "IX_Phones_CreatedAt",
                table: "Phones",
                column: "CreatedAt");

            migrationBuilder.CreateIndex(
                name: "IX_Phones_CreatedById",
                table: "Phones",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_Phones_Full_Number",
                table: "Phones",
                columns: new[] { "Ddi", "Ddd", "Number" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Phones_UpdatedById",
                table: "Phones",
                column: "UpdatedById");

            migrationBuilder.CreateIndex(
                name: "IX_ProfessionalAddresses_AddressId",
                table: "ProfessionalAddresses",
                column: "AddressId");

            migrationBuilder.CreateIndex(
                name: "IX_ProfessionalAddresses_CreatedById",
                table: "ProfessionalAddresses",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_ProfessionalAddresses_IsPrincipal",
                table: "ProfessionalAddresses",
                column: "IsPrincipal");

            migrationBuilder.CreateIndex(
                name: "IX_ProfessionalAddresses_ProfessionalId",
                table: "ProfessionalAddresses",
                column: "ProfessionalId");

            migrationBuilder.CreateIndex(
                name: "IX_ProfessionalAddresses_ProfessionalId_AddressId",
                table: "ProfessionalAddresses",
                columns: new[] { "ProfessionalId", "AddressId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ProfessionalAddresses_UpdatedById",
                table: "ProfessionalAddresses",
                column: "UpdatedById");

            migrationBuilder.CreateIndex(
                name: "IX_ProfessionalPhones_CreatedById",
                table: "ProfessionalPhones",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_ProfessionalPhones_IsPrincipal",
                table: "ProfessionalPhones",
                column: "IsPrincipal");

            migrationBuilder.CreateIndex(
                name: "IX_ProfessionalPhones_PhoneId",
                table: "ProfessionalPhones",
                column: "PhoneId");

            migrationBuilder.CreateIndex(
                name: "IX_ProfessionalPhones_ProfessionalId",
                table: "ProfessionalPhones",
                column: "ProfessionalId");

            migrationBuilder.CreateIndex(
                name: "IX_ProfessionalPhones_ProfessionalId_PhoneId",
                table: "ProfessionalPhones",
                columns: new[] { "ProfessionalId", "PhoneId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ProfessionalPhones_UpdatedById",
                table: "ProfessionalPhones",
                column: "UpdatedById");

            migrationBuilder.CreateIndex(
                name: "IX_Professional_CPF",
                table: "Professionals",
                column: "Cpf",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Professional_CreatedAt",
                table: "Professionals",
                column: "CreatedAt");

            migrationBuilder.CreateIndex(
                name: "IX_Professional_Email",
                table: "Professionals",
                column: "Email",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Professional_Name",
                table: "Professionals",
                column: "Name");

            migrationBuilder.CreateIndex(
                name: "IX_Professional_State",
                table: "Professionals",
                column: "State");

            migrationBuilder.CreateIndex(
                name: "IX_Psychologists_Crp",
                table: "Psychologists",
                column: "Crp",
                unique: true,
                filter: "[Crp] IS NOT NULL");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Nutritionists");

            migrationBuilder.DropTable(
                name: "OrganizationPhones");

            migrationBuilder.DropTable(
                name: "ProfessionalAddresses");

            migrationBuilder.DropTable(
                name: "ProfessionalPhones");

            migrationBuilder.DropTable(
                name: "Psychologists");

            migrationBuilder.DropTable(
                name: "Organizations");

            migrationBuilder.DropTable(
                name: "Addresses");

            migrationBuilder.DropTable(
                name: "Phones");

            migrationBuilder.DropTable(
                name: "Professionals");
        }
    }
}
