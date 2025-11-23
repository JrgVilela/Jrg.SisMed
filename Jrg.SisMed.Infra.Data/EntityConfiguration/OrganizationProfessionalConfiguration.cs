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
    /// Configuração do Entity Framework para a entidade OrganizationProfessional.
    /// Representa o relacionamento muitos-para-muitos entre Organization e Professional.
    /// </summary>
    public class OrganizationProfessionalConfiguration : IEntityTypeConfiguration<OrganizationProfessional>
    {
        public void Configure(EntityTypeBuilder<OrganizationProfessional> builder)
        {
            // Tabela
            builder.ToTable("OrganizationProfessionals");

            // Chave primária
            builder.HasKey(op => op.Id);

            // Propriedades
            builder.Property(op => op.Id)
                .ValueGeneratedOnAdd();

            builder.Property(op => op.OrganizationId)
                .IsRequired()
                .HasComment("ID da organização");

            builder.Property(op => op.ProfessionalId)
                .IsRequired()
                .HasComment("ID do profissional");

            builder.Property(op => op.State)
                .IsRequired()
                .HasConversion<int>()
                .HasComment("Estado da associação: 1=Active, 2=Inactive, 3=Blocked");

            // Propriedades de auditoria
            builder.Property(op => op.CreatedAt)
                .IsRequired()
                .HasDefaultValueSql("GETUTCDATE()")
                .HasComment("Data de criação do registro");

            builder.Property(op => op.UpdatedAt)
                .HasComment("Data da última atualização do registro");

            builder.Property(op => op.CreatedById)
                .HasComment("ID do profissional que criou a associação");

            builder.Property(op => op.UpdatedById)
                .HasComment("ID do profissional que atualizou a associação");

            // Relacionamentos

            // Relacionamento com Organization (muitos-para-um)
            builder.HasOne(op => op.Organization)
                .WithMany(o => o.Professionals)
                .HasForeignKey(op => op.OrganizationId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK_OrganizationProfessionals_Organizations");

            // Relacionamento com Professional (muitos-para-um)
            builder.HasOne(op => op.Professional)
                .WithMany(p => p.Organizations)
                .HasForeignKey(op => op.ProfessionalId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK_OrganizationProfessionals_Professionals");

            // Relacionamento com Professional (CreatedBy)
            builder.HasOne(op => op.CreatedBy)
                .WithMany()
                .HasForeignKey(op => op.CreatedById)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("FK_OrganizationProfessionals_CreatedBy");

            // Relacionamento com Professional (UpdatedBy)
            builder.HasOne(op => op.UpdatedBy)
                .WithMany()
                .HasForeignKey(op => op.UpdatedById)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("FK_OrganizationProfessionals_UpdatedBy");

            // Índices

            // Índice único para garantir que um profissional não seja associado mais de uma vez à mesma organização
            builder.HasIndex(op => new { op.OrganizationId, op.ProfessionalId })
                .IsUnique()
                .HasDatabaseName("IX_OrganizationProfessionals_OrgId_ProfId");

            builder.HasIndex(op => op.OrganizationId)
                .HasDatabaseName("IX_OrganizationProfessionals_OrganizationId");

            builder.HasIndex(op => op.ProfessionalId)
                .HasDatabaseName("IX_OrganizationProfessionals_ProfessionalId");

            builder.HasIndex(op => op.State)
                .HasDatabaseName("IX_OrganizationProfessionals_State");

            builder.HasIndex(op => op.CreatedAt)
                .HasDatabaseName("IX_OrganizationProfessionals_CreatedAt");

            builder.HasIndex(op => op.CreatedById)
                .HasDatabaseName("IX_OrganizationProfessionals_CreatedById");

            builder.HasIndex(op => op.UpdatedById)
                .HasDatabaseName("IX_OrganizationProfessionals_UpdatedById");
        }
    }
}
