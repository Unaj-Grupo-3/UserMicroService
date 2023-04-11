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
        public DbSet<Gender> Genders { get; set; }
        public DbSet<Image> Images { get; set; }

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
                entity.HasOne<Location>(l => l.Location)
                        .WithMany(e => e.Users)
                        .HasForeignKey(e => e.LocationId);
            });

            modelBuilder.Entity<Location>(entity =>
            {
                entity.HasKey(e => e.LocationId);
                entity.Property(e => e.LocationId).ValueGeneratedOnAdd();
                entity.Property(e => e.City).HasMaxLength(50);
                entity.Property(e => e.Province).HasMaxLength(50);
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
                entity.HasKey(e => e.GenderId);
                entity.Property(e => e.GenderId).ValueGeneratedOnAdd();
                entity.Property(e => e.Description).HasMaxLength(50);
            });
        }
    }
}
