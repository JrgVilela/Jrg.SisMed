using Jrg.SisMed.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Jrg.SisMed.Infra.Data.EntityConfiguration
{
    /// <summary>
    /// Configuração do Entity Framework para a entidade Nutritionist.
    /// Implementa o mapeamento TPT (Table per Type) herdando de Person.
    /// </summary>
    public class NutritionistConfiguration : IEntityTypeConfiguration<Nutritionist>
    {
        /// <summary>
        /// Configura o mapeamento da entidade Nutritionist para o banco de dados.
        /// </summary>
        /// <param name="builder">Builder para configuração da entidade.</param>
        public void Configure(EntityTypeBuilder<Nutritionist> builder)
        {
            // Nome da tabela específica do tipo derivado
            builder.ToTable("Nutritionists");

            // Configuração de herança: chave compartilhada com a tabela Persons
            builder.HasBaseType<Person>();

            // Propriedades específicas
            builder.Property(n => n.Crn)
                .IsRequired()
                .HasMaxLength(20)
                .HasComment("Número do registro no Conselho Regional de Nutricionistas (CRN)");

            // Índice específico para garantir unicidade do CRN
            builder.HasIndex(n => n.Crn)
                .IsUnique()
                .HasDatabaseName("IX_Nutritionists_Crn");
        }
    }
}
