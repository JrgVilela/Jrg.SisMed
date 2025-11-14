using Jrg.SisMed.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jrg.SisMed.Infra.Data.EntityConfiguration
{
    /// <summary>
    /// Configuração do Entity Framework para a entidade Organization.
    /// </summary>
    public class OrganizationConfiguration : IEntityTypeConfiguration<Organization>
    {
        public void Configure(EntityTypeBuilder<Organization> builder)
        {
            // Tabela
            builder.ToTable("Organizations");

            // Chave primária
            builder.HasKey(o => o.Id);

            // Propriedades
            builder.Property(o => o.Id)
                .ValueGeneratedOnAdd();

            builder.Property(o => o.NameFantasia)
                .IsRequired()
                .HasMaxLength(150)
                .HasComment("Nome fantasia da organização (nome comercial)");

            builder.Property(o => o.RazaoSocial)
                .IsRequired()
                .HasMaxLength(150)
                .HasComment("Razão social da organização (nome jurídico)");

            builder.Property(o => o.Cnpj)
                .IsRequired()
                .HasMaxLength(18)
                .HasComment("Cnpj da organização (apenas números)");

            builder.Property(o => o.State)
                .IsRequired()
                .HasConversion<int>()
                .HasComment("Estado da organização: 1=Active, 2=Inactive, 3=Suspended");

            // Propriedades de auditoria
            builder.Property(o => o.CreatedAt)
                .IsRequired()
                .HasDefaultValueSql("GETUTCDATE()")
                .HasComment("Data de criação do registro");

            builder.Property(o => o.UpdatedAt)
                .HasComment("Data da última atualização do registro");

            // Índices
            builder.HasIndex(o => o.Cnpj)
                .IsUnique()
                .HasDatabaseName("IX_Organizations_CNPJ");

            builder.HasIndex(o => o.State)
                .HasDatabaseName("IX_Organizations_State");

            builder.HasIndex(o => o.CreatedAt)
                .HasDatabaseName("IX_Organizations_CreatedAt");

            // Relacionamentos
            builder.HasMany(o => o.Phones)
                .WithOne(op => op.Organization)
                .HasForeignKey(op => op.OrganizationId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
