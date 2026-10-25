using Domain.Entities;
using Domain.Enums;
using Domain.Models;
using Infrastructure.Configurations;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Data
{
    public class AppDbContext: DbContext
    {
        public AppDbContext()
        {
        }

        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        public DbSet<Country> Countries { get; set; }

        public DbSet<User> Users { get; set; }

        public DbSet<Pharmacie> Pharmacies { get; set; }

        public DbSet<MedicationDetails> MedicationDetails { get; set; }

        static AppDbContext()
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfiguration(new CountryConfigurations());
            modelBuilder.ApplyConfiguration(new MedicationDetailsConfigurations());
            modelBuilder.ApplyConfiguration(new pharmacieConfigurations());
            modelBuilder.ApplyConfiguration(new UsersConfigurations());
        }
    }
}
