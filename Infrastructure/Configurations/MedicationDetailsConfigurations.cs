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
    public class MedicationDetailsConfigurations : IEntityTypeConfiguration<MedicationDetails>
    {
        public void Configure(EntityTypeBuilder<MedicationDetails> builder)
        {
            builder.ToTable("tblmedication_details");

            builder.HasKey(f => f.Id);

            builder.Property(f => f.Id)
                  .HasColumnName("id");

            builder.Property(f => f.TradeName)
                   .HasColumnName("trade_name");

            builder.Property(f => f.ScientificName)
                 .HasColumnName("scientific_name");

            builder.Property(f => f.Usage)
                  .HasColumnName("usage");

            builder.Property(f => f.Company)
                  .HasColumnName("company");

            builder.Property(f => f.Agent)
                  .HasColumnName("agent");

            builder.Property(f => f.PharmaceuticalForm)
                  .HasColumnName("pharmaceutical_form");

            builder.Property(f => f.Price)
                  .HasColumnName("price");

            builder.Property(f => f.AgentsPhoneNumber)
                  .HasColumnName("agents_phone_number");

            builder.Property(f => f.CountryId)
                  .HasColumnName("country_id");

            builder.HasOne(f => f.Country)
                   .WithMany()
                   .HasForeignKey(f => f.CountryId)
                   .IsRequired(true); 
        }
    }
}
