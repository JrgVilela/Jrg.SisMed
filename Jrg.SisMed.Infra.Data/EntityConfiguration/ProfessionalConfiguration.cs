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
    /// Configuração do Entity Framework para a entidade Professional.
    /// </summary>
    public class ProfessionalConfiguration : IEntityTypeConfiguration<Professional>
    {
        public void Configure(EntityTypeBuilder<Professional> builder)
        {
            // Tabela
            builder.ToTable("Professionals");

            // Chave primária
            builder.HasKey(p => p.Id);

            // Propriedades
            builder.Property(p => p.Id)
                .ValueGeneratedOnAdd();

            builder.Property(p => p.Name)
                .IsRequired()
                .HasMaxLength(150)
                .HasComment("Nome completo do Professional");

            builder.Property(p => p.Cpf)
                .IsRequired()
                .HasMaxLength(14)
                .HasComment("CPF do profissional (apenas números)");

            builder.Property(p => p.Rg)
                .HasMaxLength(20)
                .HasComment("RG do profissional");

            builder.Property(p => p.BirthDate)
                .HasComment("Data de nascimento");

            builder.Property(p => p.Gender)
                .IsRequired()
                .HasConversion<int>()
                .HasComment("Gênero: 0=None, 1=Male, 2=Female, 3=Other");

            builder.Property(p => p.State)
                .IsRequired()
                .HasConversion<int>()
                .HasComment("Estado do profissional: 1=Active, 2=Inactive");

            // Propriedades de auditoria
            builder.Property(p => p.CreatedAt)
                .IsRequired()
                .HasDefaultValueSql("GETUTCDATE()")
                .HasComment("Data de criação do registro");

            builder.Property(p => p.UpdatedAt)
                .HasComment("Data da última atualização do registro");

            // Índices
            builder.HasIndex(p => p.Cpf)
                .IsUnique()
                .HasDatabaseName("IX_Professional_CPF");

            builder.HasIndex(p => p.State)
                .HasDatabaseName("IX_Professional_State");

            builder.HasIndex(p => p.Name)
                .HasDatabaseName("IX_Professional_Name");

            builder.HasIndex(p => p.CreatedAt)
                .HasDatabaseName("IX_Professional_CreatedAt");

            // Relacionamentos
            builder.HasMany(p => p.Addresses)
                .WithOne(pa => pa.Professional)
                .HasForeignKey(pa => pa.ProfessionalId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasMany(p => p.Phones)
                .WithOne(pp => pp.Professional)
                .HasForeignKey(pp => pp.ProfessionalId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
