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
    /// Configuração do Entity Framework para a entidade Phone.
    /// </summary>
    public class PhoneConfiguration : IEntityTypeConfiguration<Phone>
    {
        public void Configure(EntityTypeBuilder<Phone> builder)
        {
            // Tabela
            builder.ToTable("Phones");

            // Chave primária
            builder.HasKey(p => p.Id);

            // Propriedades
            builder.Property(p => p.Id)
                .ValueGeneratedOnAdd();

            builder.Property(p => p.Ddi)
                .IsRequired()
                .HasMaxLength(3)
                .HasComment("Código DDI (código internacional do país)");

            builder.Property(p => p.Ddd)
                .IsRequired()
                .HasMaxLength(2)
                .HasComment("Código DDD (código de área)");

            builder.Property(p => p.Number)
                .IsRequired()
                .HasMaxLength(9)
                .HasComment("Número do telefone (8 ou 9 dígitos)");

            // Propriedades de auditoria (EntityBase)
            builder.Property(p => p.CreatedById)
                .HasComment("ID da pessoa que criou o registro");

            builder.Property(p => p.UpdatedById)
                .HasComment("ID da pessoa que atualizou o registro");

            builder.Property(p => p.CreatedAt)
                .IsRequired()
                .HasDefaultValueSql("GETUTCDATE()")
                .HasComment("Data de criação do registro");

            builder.Property(p => p.UpdatedAt)
                .HasComment("Data da última atualização do registro");

            // Índices
            builder.HasIndex(p => new { p.Ddi, p.Ddd, p.Number })
                .IsUnique()
                .HasDatabaseName("IX_Phones_Full_Number");

            builder.HasIndex(p => p.CreatedAt)
                .HasDatabaseName("IX_Phones_CreatedAt");

            // Relacionamentos com Professional (CreatedBy, UpdatedBy)
            builder.HasOne(p => p.CreatedBy)
                .WithMany()
                .HasForeignKey(p => p.CreatedById)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(p => p.UpdatedBy)
                .WithMany()
                .HasForeignKey(p => p.UpdatedById)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
