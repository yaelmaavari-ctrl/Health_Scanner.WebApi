using Microsoft.EntityFrameworkCore;
using Repository.Entities;
using Repository.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataContext
{
    public class HealthScannerContext(DbContextOptions<HealthScannerContext> options) : DbContext(options), IContext
    {
        public DbSet<User> Users { get; set; }
        public DbSet<Allergen> Allergens { get; set; }
        public DbSet<UserAllergen> UserAllergens { get; set; }
        public DbSet<ScanHistory> ScanHistories { get; set; }

        public async Task Save()
        {
            await SaveChangesAsync();
        }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<UserAllergen>()
                .HasKey(ua => new { ua.UserId, ua.AllergenId });

            modelBuilder.Entity<UserAllergen>()
                .HasOne(ua => ua.User)
                .WithMany(u => u.UserAllergens)
                .HasForeignKey(ua => ua.UserId);

            modelBuilder.Entity<UserAllergen>()
                .HasOne(ua => ua.Allergen)
                .WithMany() 
                .HasForeignKey(ua => ua.AllergenId);
        }
    }
}
