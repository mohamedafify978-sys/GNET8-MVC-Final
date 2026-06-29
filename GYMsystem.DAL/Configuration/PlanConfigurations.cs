using GYMsystem.DAL.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GYMsystem.DAL.Configuration
{
    public class PlanConfigurations : IEntityTypeConfiguration<Plan>

    {
        public void Configure(EntityTypeBuilder<Plan> builder)
        {
            builder.Property(X => X.Name)
               .HasColumnType("varchar")
               .HasMaxLength(50);

            builder.Property(X => X.Description)
                   .HasMaxLength(200);
            
            builder.Property(X => X.Price)
                .HasPrecision(10, 2);

            builder.Property(X => X.CreatedAt)
                   .HasDefaultValueSql("GETDATE()");

            builder.ToTable(Tb =>
            {
                Tb.HasCheckConstraint("PlanDurationCheck",
                    "DurationDays Between 1 and 365");
            });
        }
    }
}
