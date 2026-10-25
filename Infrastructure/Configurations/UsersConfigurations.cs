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
    public class UsersConfigurations : IEntityTypeConfiguration<User>
    {
        public void Configure(EntityTypeBuilder<User> builder)
        {
            builder.ToTable("tblusers");

            builder.HasKey(f => f.Id);

            builder.Property(f => f.Id)
                  .HasColumnName("id");

            builder.Property(f => f.Name)
                   .HasColumnName("name");

            builder.Property(f => f.PhoneNumber)
                 .HasColumnName("phone_number");

            builder.Property(f => f.Role)
                  .HasColumnName("role")
                  .HasColumnType("user_role"); ;

            builder.Property(f => f.ActivationCode)
                  .HasColumnName("activation_code");

            builder.Property(f => f.Email)
                  .HasColumnName("email")
                  .IsRequired(false);

            builder.HasIndex(f => f.Email)
                   .IsUnique();

            builder.Property(f => f.Password)
                  .HasColumnName("password");

            builder.Property(f => f.AgentName)
                  .HasColumnName("agent_name");

            builder.Property(f => f.CountryId)
                  .HasColumnName("country_id");

            builder.Property(f => f.DeviceIp)
                 .HasColumnName("device_ip");

            builder.HasOne(f => f.Country)
                   .WithMany()
                   .HasForeignKey(f => f.CountryId)
                   .IsRequired(true);
        }
    }
}
