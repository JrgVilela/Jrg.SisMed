using Jrg.SisMed.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jrg.SisMed.Infra.Data.Context
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

        public DbSet<Address> Addresses { get; set; }
        public DbSet<Phone> Phones { get; set; }
        public DbSet<Professional> Persons { get; set; }
        public DbSet<Psychologist> Psychologists { get; set; }
        public DbSet<Nutritionist> Nutritionists { get; set; }
        public DbSet<Organization> Organizations { get; set; }
        public DbSet<OrganizationPhone> OrganizationPhones { get; set; }
        public DbSet<ProfessionalAddress> PersonAddresses { get; set; }
        public DbSet<ProfessionalPhone> PersonPhones { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            builder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);
        }
    }
}
