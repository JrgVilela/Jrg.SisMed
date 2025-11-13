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
    /// Configuração do Entity Framework para a entidade ProfessionalPhone (tabela de relacionamento).
    /// </summary>
    public class ProfessionalPhoneConfiguration : IEntityTypeConfiguration<ProfessionalPhone>
    {
        public void Configure(EntityTypeBuilder<ProfessionalPhone> builder)
        {
            // Tabela
            builder.ToTable("ProfessionalPhones");

            // Chave primária
            builder.HasKey(pp => pp.Id);

            // Propriedades
            builder.Property(pp => pp.Id)
                .ValueGeneratedOnAdd();

            builder.Property(pp => pp.ProfessionalId)
                .IsRequired()
                .HasComment("ID o profissional");

            builder.Property(pp => pp.PhoneId)
                .IsRequired()
                .HasComment("ID do telefone");

            builder.Property(pp => pp.IsPrincipal)
                .IsRequired()
                .HasDefaultValue(false)
                .HasComment("Indica se é o telefone principal");

            // Propriedades de auditoria (EntityBase)
            builder.Property(pp => pp.CreatedById)
                .HasComment("ID do profissional que criou o registro");

            builder.Property(pp => pp.UpdatedById)
                .HasComment("ID do profissional que atualizou o registro");

            builder.Property(pp => pp.CreatedAt)
                .IsRequired()
                .HasDefaultValueSql("GETUTCDATE()")
                .HasComment("Data de criação do registro");

            builder.Property(pp => pp.UpdatedAt)
                .HasComment("Data da última atualização do registro");

            // Índices
            builder.HasIndex(pp => new { pp.ProfessionalId, pp.PhoneId })
                .IsUnique()
                .HasDatabaseName("IX_ProfessionalPhones_ProfessionalId_PhoneId");

            builder.HasIndex(pp => pp.ProfessionalId)
                .HasDatabaseName("IX_ProfessionalPhones_ProfessionalId");

            builder.HasIndex(pp => pp.PhoneId)
                .HasDatabaseName("IX_ProfessionalPhones_PhoneId");

            builder.HasIndex(pp => pp.IsPrincipal)
                .HasDatabaseName("IX_ProfessionalPhones_IsPrincipal");

            // Relacionamentos
            builder.HasOne(pp => pp.Professional)
                .WithMany(p => p.Phones)
                .HasForeignKey(pp => pp.ProfessionalId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(pp => pp.Phone)
                .WithMany()
                .HasForeignKey(pp => pp.PhoneId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(pp => pp.CreatedBy)
                .WithMany()
                .HasForeignKey(pp => pp.CreatedById)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(pp => pp.UpdatedBy)
                .WithMany()
                .HasForeignKey(pp => pp.UpdatedById)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
