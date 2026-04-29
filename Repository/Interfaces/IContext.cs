using Microsoft.EntityFrameworkCore;
using Repository.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository.Interfaces
{
    public interface IContext
    {
        public DbSet<User> Users { get; set; }
        public DbSet<Allergen> Allergens { get; set; }
        public DbSet<UserAllergen> UserAllergens { get; set; }
        public DbSet<ScanHistory> ScanHistories { get; set; }
        public Task Save();
    }
}
