using Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Configurations
{
    public class CountryConfigurations : IEntityTypeConfiguration<Country>
    {
        public void Configure(EntityTypeBuilder<Country> builder)
        {
            builder.ToTable("tblcountries");

            builder.HasKey(c => c.Id);

            builder.Property(f => f.Id)
                  .HasColumnName("id");

            builder.Property(f => f.Name)
                   .HasColumnName("name");
        }
    }
}
