using FlintSoftFood.Contracts.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace FlintSoftFood.DAL.Infrastructure
{
    public class FoodContext : DbContext
    {
        public DbSet<Food> Food { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            foreach (IMutableEntityType entity in modelBuilder.Model.GetEntityTypes())
            {
                entity.SetTableName("Food_" + entity.GetTableName());

            }
        }
    }
}
