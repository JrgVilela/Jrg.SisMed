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
    /// Configuração do Entity Framework para a entidade Address.
    /// </summary>
    public class AddressConfiguration : IEntityTypeConfiguration<Address>
    {
        public void Configure(EntityTypeBuilder<Address> builder)
        {
            // Tabela
            builder.ToTable("Addresses");

            // Chave primária
            builder.HasKey(a => a.Id);

            // Propriedades
            builder.Property(a => a.Id)
                .ValueGeneratedOnAdd();

            builder.Property(a => a.Street)
                .IsRequired()
                .HasMaxLength(200)
                .HasComment("Nome da rua/logradouro");

            builder.Property(a => a.Number)
                .IsRequired()
                .HasMaxLength(20)
                .HasComment("Número do endereço");

            builder.Property(a => a.Complement)
                .HasMaxLength(100)
                .HasComment("Complemento do endereço");

            builder.Property(a => a.ZipCode)
                .IsRequired()
                .HasMaxLength(8)
                .HasComment("CEP (apenas números)");

            builder.Property(a => a.City)
                .IsRequired()
                .HasMaxLength(100)
                .HasComment("Cidade");

            builder.Property(a => a.State)
                .IsRequired()
                .HasMaxLength(2)
                .HasComment("Estado/UF (sigla)");

            // Propriedades de auditoria (EntityBase)
            builder.Property(a => a.CreatedById)
                .HasComment("ID da pessoa que criou o registro");

            builder.Property(a => a.UpdatedById)
                .HasComment("ID da pessoa que atualizou o registro");

            builder.Property(a => a.CreatedAt)
                .IsRequired()
                .HasDefaultValueSql("GETUTCDATE()")
                .HasComment("Data de criação do registro");

            builder.Property(a => a.UpdatedAt)
                .HasComment("Data da última atualização do registro");

            // Índices
            builder.HasIndex(a => a.ZipCode)
                .HasDatabaseName("IX_Addresses_ZipCode");

            builder.HasIndex(a => new { a.City, a.State })
                .HasDatabaseName("IX_Addresses_City_State");

            builder.HasIndex(a => a.CreatedAt)
                .HasDatabaseName("IX_Addresses_CreatedAt");

            // Relacionamentos com Person (CreatedBy, UpdatedBy)
            builder.HasOne(a => a.CreatedBy)
                .WithMany()
                .HasForeignKey(a => a.CreatedById)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(a => a.UpdatedBy)
                .WithMany()
                .HasForeignKey(a => a.UpdatedById)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
