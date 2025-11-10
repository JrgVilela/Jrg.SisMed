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
    public class PersonConfiguration : IEntityTypeConfiguration<Professional>
    {
        public void Configure(EntityTypeBuilder<Professional> builder)
        {
            // Tabela
            builder.ToTable("Persons");

            // Chave primária
            builder.HasKey(p => p.Id);

            // Propriedades
            builder.Property(p => p.Id)
                .ValueGeneratedOnAdd();

            builder.Property(p => p.Name)
                .IsRequired()
                .HasMaxLength(150)
                .HasComment("Nome completo da pessoa");

            builder.Property(p => p.Cpf)
                .IsRequired()
                .HasMaxLength(11)
                .HasComment("CPF da pessoa (apenas números)");

            builder.Property(p => p.Rg)
                .HasMaxLength(20)
                .HasComment("RG da pessoa");

            builder.Property(p => p.BirthDate)
                .HasComment("Data de nascimento");

            builder.Property(p => p.Gender)
                .IsRequired()
                .HasConversion<int>()
                .HasComment("Gênero: 0=None, 1=Male, 2=Female, 3=Other");

            builder.Property(p => p.State)
                .IsRequired()
                .HasConversion<int>()
                .HasComment("Estado da pessoa: 1=Active, 2=Inactive");

            builder.Property(p => p.Email)
                .IsRequired()
                .HasMaxLength(100)
                .HasComment("E-mail da pessoa");

            builder.Property(p => p.PasswordHash)
                .IsRequired()
                .HasMaxLength(500)
                .HasComment("Hash da senha (PBKDF2)");

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
                .HasDatabaseName("IX_Persons_CPF");

            builder.HasIndex(p => p.Email)
                .IsUnique()
                .HasDatabaseName("IX_Persons_Email");

            builder.HasIndex(p => p.State)
                .HasDatabaseName("IX_Persons_State");

            builder.HasIndex(p => p.Name)
                .HasDatabaseName("IX_Persons_Name");

            builder.HasIndex(p => p.CreatedAt)
                .HasDatabaseName("IX_Persons_CreatedAt");

            // Relacionamentos
            builder.HasMany(p => p.Addresses)
                .WithOne(pa => pa.Professional)
                .HasForeignKey(pa => pa.PersonId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasMany(p => p.Phones)
                .WithOne(pp => pp.Person)
                .HasForeignKey(pp => pp.PersonId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
