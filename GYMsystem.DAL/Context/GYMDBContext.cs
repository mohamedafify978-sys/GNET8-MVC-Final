using GYMsystem.DAL.Configuration;
using GYMsystem.DAL.Configurations;
using GYMsystem.DAL.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace GYMsystem.DAL.Context
{
    public class GYMDBContext : IdentityDbContext<ApplicationUser>
    {
        //protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        //{
        //    optionsBuilder.UseSqlServer(
        //   "Server=.;Database=GYM;Trusted_Connection=True;TrustServerCertificate=True");
        //}

        public GYMDBContext(DbContextOptions<GYMDBContext> options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());

            modelBuilder.Entity<HealthRecord>()
                .Property(x => x.Height)
                .HasPrecision(5, 2);

            modelBuilder.Entity<HealthRecord>()
                .Property(x => x.Weight)
                .HasPrecision(5, 2);

            modelBuilder.Entity<ApplicationUser>(EB =>
            {
                EB.Property(X => X.FirstName)
                .HasColumnType("varchar")
                .HasMaxLength(50);

                EB.Property(X => X.LastName)
                .HasColumnType("varchar")
                .HasMaxLength(50);
            });


        }


        public DbSet<Plan> Plans { get; set; }
        public DbSet<Member> Members { get; set; }
        public DbSet<Membership> Memberships { get; set; }
        public DbSet<Booking> Bookings { get; set; }
        public DbSet<Session> Sessions { get; set; }
        public DbSet<Trainer> Trainers { get; set; }
        public DbSet<Category> Categories { get; set; }

        public DbSet<HealthRecord> HealthRecords { get; set; }
    


        }

}
