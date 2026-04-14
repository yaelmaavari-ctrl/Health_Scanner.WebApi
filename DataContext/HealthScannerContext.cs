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

        public async Task Save()
        {
            await SaveChangesAsync();
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<UserAllergen>().HasKey(ua => new { ua.UserId, ua.AllergenId });
        }
    }
}
