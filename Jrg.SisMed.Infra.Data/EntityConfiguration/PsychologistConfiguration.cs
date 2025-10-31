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
    /// Configuração do Entity Framework para a entidade Psychologist.
    /// Implementa o mapeamento TPT (Table per Type) herdando de Person.
    /// </summary>
    public class PsychologistConfiguration : IEntityTypeConfiguration<Psychologist>
    {
        public void Configure(EntityTypeBuilder<Psychologist> builder)
        {
            // Nome da tabela específica do tipo derivado
            builder.ToTable("Psychologists");

            // Configuração de herança: chave compartilhada com a tabela Persons
            builder.HasBaseType<Person>();

            // Propriedades específicas
            builder.Property(p => p.Crp)
                .IsRequired()
                .HasMaxLength(20)
                .HasComment("Número do registro no Conselho Regional de Psicologia (CRP)");

            // Índice específico (se desejar garantir unicidade do CRP)
            builder.HasIndex(p => p.Crp)
                .IsUnique()
                .HasDatabaseName("IX_Psychologists_Crp");
        }
    }
}
