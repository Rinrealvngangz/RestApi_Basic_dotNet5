using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RestAPI_Food.Etites;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RestAPI_Food.configuration
{
    public class FoodConfiguration : IEntityTypeConfiguration<Food>
    {
        public void Configure(EntityTypeBuilder<Food> builder)
        {
            builder.Property(x => x.Name).IsRequired();
            builder.Property(x => x.Price).IsRequired();
   
             builder.HasOne(f => f.Category)
                    .WithMany(c => c.Foods).HasForeignKey(c => c.CategoryId)
                    .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
