using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Persistance
{
    public class ExpresoDbContext : DbContext
    {
        public DbSet<User> Users { get; set; }
        public DbSet<Location> Locations { get; set; }
        public DbSet<Image> Images { get; set; }
        public DbSet<City> Cities { get; set; }
        public DbSet<Province> Provinces { get; set; }
        public DbSet <Gender> Gender { get; set; }

        public ExpresoDbContext(DbContextOptions<ExpresoDbContext> options ) : base(options)
        {

        }

        public ExpresoDbContext() { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>(entity =>
            {
                entity.HasKey(e => e.UserId);
                entity.Property(e => e.UserId).ValueGeneratedOnAdd();
                entity.Property(e => e.Name).HasMaxLength(50);
                entity.Property(e => e.LastName).HasMaxLength(50);
                entity.Property(e => e.Description).HasMaxLength(255);
                entity.Property(e => e.Birthday).HasColumnType("date");
                entity.HasOne<Location>(l => l.Location)
                        .WithMany(e => e.Users)
                        .HasForeignKey(e => e.LocationId);

                entity.HasOne<Gender>(g => g.Gender)
                        .WithOne(h => h.User).HasForeignKey<User>(x => x.GenderId);

            });


            modelBuilder.Entity<Location>(entity =>
            {
                entity.HasKey(e => e.LocationId);
                entity.Property(e => e.LocationId).ValueGeneratedOnAdd();
                
                entity.HasOne<City>(e => e.City)
                        .WithMany(e => e.Location)
                        .HasForeignKey(e => e.CityId);

                entity.HasOne<Province>(e => e.Province)
                        .WithMany(e => e.Location)
                        .HasForeignKey(e => e.ProvinceId);
            });

            modelBuilder.Entity<City>(entity =>
            {
                entity.HasKey(e => e.CityId);
                entity.Property(e => e.CityId).ValueGeneratedOnAdd();
            });

            modelBuilder.Entity<Province>(entity =>
            {
                entity.HasKey(e => e.ProvinceId);
                entity.Property(e => e.ProvinceId).ValueGeneratedOnAdd();
            });

            modelBuilder.Entity<Image>(entity =>
            {
                entity.HasKey(e => e.ImageId);
                entity.Property(e => e.ImageId).ValueGeneratedOnAdd();
                entity.HasOne<User>(e => e.User)
                    .WithMany(e => e.Images)
                    .HasForeignKey(e => e.UserId);
            });

            modelBuilder.Entity<Gender>(entity =>
            {
                entity.HasKey(k => k.GenderId);
                entity.Property(p => p.Description).HasMaxLength(255);
                entity.HasOne<User>(u => u.User).WithOne(w => w.Gender);

                entity.HasData(new Gender
                {
                    GenderId= 1,
                    Description = "Masculino"
                });

                entity.HasData(new Gender
                {
                    GenderId = 2,
                    Description = "Femenino"
                });

                entity.HasData(new Gender
                {
                    GenderId = 3,
                    Description = "Otros"
                });

            });
        }
    }
}
