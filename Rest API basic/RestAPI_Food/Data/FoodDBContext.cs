using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using RestAPI_Food.Etites;
using RestAPI_Food.configuration;
namespace RestAPI_Food.Data
{
    public class FoodDBContext : DbContext
    {
        public FoodDBContext(DbContextOptions options) : base(options)
        {

        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
          
            modelBuilder.ApplyConfiguration(new FoodConfiguration());

        }

        public DbSet<Food> Foods { get; set; }

        public DbSet<Category> Categories { get; set; }
    }
}
