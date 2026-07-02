using GYMsystem.DAL.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GYMsystem.DAL.Configurations
{
    internal class CategoryConfigurations : IEntityTypeConfiguration<Category>
    {
        public void Configure(EntityTypeBuilder<Category> builder)
        {
            builder.Property(X => X.CategoryName)
                .HasColumnType("varchar")
                .HasMaxLength(20);

            builder.HasData(
                             new Category { Id = 1, CategoryName = "Cardio" },
                             new Category { Id = 2, CategoryName = "Strength" },
                             new Category { Id = 3, CategoryName = "Yoga" },
                             new Category { Id = 4, CategoryName = "Boxing" },
                             new Category { Id = 5, CategoryName = "CrossFit" },
                             new Category { Id = 6, CategoryName = "GeneralFitness" }
                         );

        }
    }
}
