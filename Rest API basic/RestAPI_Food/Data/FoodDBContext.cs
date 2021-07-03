using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using RestAPI_Food.Etites;
using RestAPI_Food.configuration;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using System.Diagnostics.CodeAnalysis;
using RestAPI_Food.Etities;

namespace RestAPI_Food.Data
{
    public class FoodDBContext : IdentityDbContext
    {
        public FoodDBContext(DbContextOptions<FoodDBContext> options) : base(options)
        {

        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
                      
            modelBuilder.ApplyConfiguration(new FoodConfiguration());
            base.OnModelCreating(modelBuilder);
        }

        public  DbSet<Food> Foods { get; set; }

        public  DbSet<Category> Categories { get; set; }

        public DbSet<RefreshToken> RefreshTokens { get; set; }
    }
}
