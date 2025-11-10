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
    /// Configuração do Entity Framework para a entidade ProfessionalAddress (tabela de relacionamento).
    /// </summary>
    public class PersonAddressConfiguration : IEntityTypeConfiguration<ProfessionalAddress>
    {
        public void Configure(EntityTypeBuilder<ProfessionalAddress> builder)
        {
            // Tabela
            builder.ToTable("PersonAddresses");

            // Chave primária
            builder.HasKey(pa => pa.Id);

            // Propriedades
            builder.Property(pa => pa.Id)
                .ValueGeneratedOnAdd();

            builder.Property(pa => pa.PersonId)
                .IsRequired()
                .HasComment("ID da pessoa");

            builder.Property(pa => pa.AddressId)
                .IsRequired()
                .HasComment("ID do endereço");

            builder.Property(pa => pa.IsPrincipal)
                .IsRequired()
                .HasDefaultValue(false)
                .HasComment("Indica se é o endereço principal");

            // Propriedades de auditoria (EntityBase)
            builder.Property(pa => pa.CreatedById)
                .HasComment("ID da pessoa que criou o registro");

            builder.Property(pa => pa.UpdatedById)
                .HasComment("ID da pessoa que atualizou o registro");

            builder.Property(pa => pa.CreatedAt)
                .IsRequired()
                .HasDefaultValueSql("GETUTCDATE()")
                .HasComment("Data de criação do registro");

            builder.Property(pa => pa.UpdatedAt)
                .HasComment("Data da última atualização do registro");

            // Índices
            builder.HasIndex(pa => new { pa.PersonId, pa.AddressId })
                .IsUnique()
                .HasDatabaseName("IX_PersonAddresses_PersonId_AddressId");

            builder.HasIndex(pa => pa.PersonId)
                .HasDatabaseName("IX_PersonAddresses_PersonId");

            builder.HasIndex(pa => pa.AddressId)
                .HasDatabaseName("IX_PersonAddresses_AddressId");

            builder.HasIndex(pa => pa.IsPrincipal)
                .HasDatabaseName("IX_PersonAddresses_IsPrincipal");

            // Relacionamentos
            builder.HasOne(pa => pa.Professional)
                .WithMany(p => p.Addresses)
                .HasForeignKey(pa => pa.PersonId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(pa => pa.Address)
                .WithMany()
                .HasForeignKey(pa => pa.AddressId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(pa => pa.CreatedBy)
                .WithMany()
                .HasForeignKey(pa => pa.CreatedById)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(pa => pa.UpdatedBy)
                .WithMany()
                .HasForeignKey(pa => pa.UpdatedById)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
