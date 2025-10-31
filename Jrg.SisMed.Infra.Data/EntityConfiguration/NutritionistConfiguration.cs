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
    public class NutritionistConfiguration : IEntityTypeConfiguration<Nutritionist>
    {
        public void Configure(EntityTypeBuilder<Nutritionist> builder)
        {
            builder.ToTable("Nutritionists");

            builder.HasBaseType<Person>();

            builder.Property(n => n.Crn)
                .IsRequired()
                .HasMaxLength(20)
                .HasComment("Número de registro no conselho regional de nutricionista (CRN)");

            builder.HasIndex(n => n.Crn)
                .IsUnique()
                .HasDatabaseName("IX_Nutritionists_Crn");
        }
    }
}
