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
    /// Configuração do Entity Framework para a entidade OrganizationPhone (tabela de relacionamento).
    /// </summary>
    public class OrganizationPhoneConfiguration : IEntityTypeConfiguration<OrganizationPhone>
    {
        public void Configure(EntityTypeBuilder<OrganizationPhone> builder)
        {
            // Tabela
            builder.ToTable("OrganizationPhones");

            // Chave primária
            builder.HasKey(op => op.Id);

            // Propriedades
            builder.Property(op => op.Id)
                .ValueGeneratedOnAdd();

            builder.Property(op => op.OrganizationId)
                .IsRequired()
                .HasComment("ID da organização");

            builder.Property(op => op.PhoneId)
                .IsRequired()
                .HasComment("ID do telefone");

            builder.Property(op => op.IsPrincipal)
                .IsRequired()
                .HasDefaultValue(false)
                .HasComment("Indica se é o telefone principal");

            // Propriedades de auditoria (EntityBase)
            builder.Property(op => op.CreatedById)
                .HasComment("ID da pessoa que criou o registro");

            builder.Property(op => op.UpdatedById)
                .HasComment("ID da pessoa que atualizou o registro");

            builder.Property(op => op.CreatedAt)
                .IsRequired()
                .HasDefaultValueSql("GETUTCDATE()")
                .HasComment("Data de criação do registro");

            builder.Property(op => op.UpdatedAt)
                .HasComment("Data da última atualização do registro");

            // Índices
            builder.HasIndex(op => new { op.OrganizationId, op.PhoneId })
                .IsUnique()
                .HasDatabaseName("IX_OrganizationPhones_OrganizationId_PhoneId");

            builder.HasIndex(op => op.OrganizationId)
                .HasDatabaseName("IX_OrganizationPhones_OrganizationId");

            builder.HasIndex(op => op.PhoneId)
                .HasDatabaseName("IX_OrganizationPhones_PhoneId");

            builder.HasIndex(op => op.IsPrincipal)
                .HasDatabaseName("IX_OrganizationPhones_IsPrincipal");

            // Relacionamentos
            builder.HasOne(op => op.Organization)
                .WithMany(o => o.Phones)
                .HasForeignKey(op => op.OrganizationId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(op => op.Phone)
                .WithMany()
                .HasForeignKey(op => op.PhoneId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(op => op.CreatedBy)
                .WithMany()
                .HasForeignKey(op => op.CreatedById)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(op => op.UpdatedBy)
                .WithMany()
                .HasForeignKey(op => op.UpdatedById)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
