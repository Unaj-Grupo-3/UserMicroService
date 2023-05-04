using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistance
{
    public class ExpresoDbContext : DbContext
    {
        public DbSet<UserProfile> Users { get; set; }
        public DbSet<Location> Locations { get; set; }
        public DbSet<Image> Images { get; set; }
        public DbSet <Gender> Gender { get; set; }

        public ExpresoDbContext(DbContextOptions<ExpresoDbContext> options ) : base(options)
        {

        }

        public ExpresoDbContext() { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<UserProfile>(entity =>
            {
                entity.HasKey(e => e.UserId);
                entity.Property(e => e.Name).HasMaxLength(50);
                entity.Property(e => e.LastName).HasMaxLength(50);
                entity.Property(e => e.Description).HasMaxLength(255);
                entity.Property(e => e.Birthday).HasColumnType("date");

                entity.HasOne<Location>(l => l.Location)
                      .WithMany(e => e.Users)
                      .HasForeignKey(e => e.LocationId);

                entity.HasOne<Gender>(e => e.Gender)
                      .WithMany(e => e.Users)
                      .HasForeignKey(e => e.GenderId);

                entity.HasOne<Location>(e => e.Location)
                      .WithMany(e => e.Users)
                      .HasForeignKey(e => e.LocationId);
            });


            modelBuilder.Entity<Location>(entity =>
            {
                entity.HasKey(e => e.LocationId);
                entity.Property(e => e.LocationId).ValueGeneratedOnAdd();
                entity.Property(e => e.Country).HasMaxLength(50).IsRequired();
                entity.Property(e => e.Province).HasMaxLength(50).IsRequired();
                entity.Property(e => e.City).HasMaxLength(50).IsRequired();
                entity.Property(e => e.Latitude).IsRequired();
                entity.Property(e => e.Longitude).IsRequired();
                
            });

            modelBuilder.Entity<Image>(entity =>
            {
                entity.HasKey(e => e.ImageId);
                entity.Property(e => e.ImageId).ValueGeneratedOnAdd();


                entity.HasOne<UserProfile>(e => e.User)
                      .WithMany(e => e.Images)
                      .HasForeignKey(e => e.UserId);
            });

            modelBuilder.Entity<Gender>(entity =>
            {
                entity.HasKey(k => k.GenderId);
                entity.Property(p => p.Description).HasMaxLength(255);

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
