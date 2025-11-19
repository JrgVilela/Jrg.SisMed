using Jrg.SisMed.Domain.Entities;
using Jrg.SisMed.Domain.Helpers;
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
    /// Configuração do Entity Framework para a entidade User.
    /// </summary>
    public class UserConfiguration : IEntityTypeConfiguration<User>
    {
        public void Configure(EntityTypeBuilder<User> builder)
        {
            // Tabela
            builder.ToTable("Users");

            // Chave primária
            builder.HasKey(u => u.Id);

            // Propriedades
            builder.Property(u => u.Id)
                .ValueGeneratedOnAdd();

            builder.Property(u => u.Name)
                .IsRequired()
                .HasMaxLength(100)
                .HasComment("Nome completo do usuário");

            builder.Property(u => u.Email)
                .IsRequired()
                .HasMaxLength(100)
                .HasComment("E-mail do usuário (único)");

            builder.Property(u => u.Password)
                .IsRequired()
                .HasMaxLength(500)
                .HasComment("Senha do usuário (hash PBKDF2)");

            builder.Property(u => u.State)
                .IsRequired()
                .HasConversion<int>()
                .HasComment("Estado do usuário: 1=Active, 2=Inactive, 3=Blocked");

            // Propriedades de auditoria
            builder.Property(u => u.CreatedAt)
                .IsRequired()
                .HasDefaultValueSql("GETUTCDATE()")
                .HasComment("Data de criação do registro");

            builder.Property(u => u.UpdatedAt)
                .HasComment("Data da última atualização do registro");

            // Índices
            builder.HasIndex(u => u.Email)
                .IsUnique()
                .HasDatabaseName("IX_Users_Email");

            builder.HasIndex(u => u.State)
                .HasDatabaseName("IX_Users_State");

            builder.HasIndex(u => u.CreatedAt)
                .HasDatabaseName("IX_Users_CreatedAt");
        }
    }
}
