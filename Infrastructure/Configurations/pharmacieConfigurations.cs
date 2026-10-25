using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Configurations
{
    public class pharmacieConfigurations : IEntityTypeConfiguration<Pharmacie>
    {
        public void Configure(EntityTypeBuilder<Pharmacie> builder)
        {
            builder.ToTable("tblpharmacies");

            builder.HasKey(f => f.Id);

            builder.Property(f => f.Id)
                  .HasColumnName("id");

            builder.Property(f => f.Name)
                   .HasColumnName("name");

            builder.Property(f => f.Address)
                 .HasColumnName("address");

            builder.Property(f => f.ManagerName)
                  .HasColumnName("manager_name");

            builder.Property(f => f.PhoneNumber)
                  .HasColumnName("phone_number");

            builder.Property(f => f.CountryId)
                  .HasColumnName("country_id");

            builder.HasOne(f => f.Country)
                   .WithMany()
                   .HasForeignKey(f => f.CountryId)
                   .IsRequired(true);
        }
    }
}

