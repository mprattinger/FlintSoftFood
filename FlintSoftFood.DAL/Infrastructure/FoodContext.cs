using FlintSoftFood.Contracts.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Metadata;
using System;
using System.Linq;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;


namespace FlintSoftFood.DAL.Infrastructure
{
    public class FoodContext : DbContext
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public DbSet<Food> Food { get; set; }

        public FoodContext(DbContextOptions<FoodContext> options, IHttpContextAccessor httpContextAccessor) : base(options)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            foreach (IMutableEntityType entity in modelBuilder.Model.GetEntityTypes())
            {
                entity.SetTableName("Food_" + entity.GetTableName());

            }

            foreach (var entityType in modelBuilder.Model.GetEntityTypes().Where(t => typeof(IAuditable).IsAssignableFrom(t.ClrType)))
            {
                modelBuilder.Entity(entityType.ClrType).Property<DateTime>("CreatedAt");
                modelBuilder.Entity(entityType.ClrType).Property<DateTime>("ChangedAt");
                modelBuilder.Entity(entityType.ClrType).Property<string>("CreatedBy");
                modelBuilder.Entity(entityType.ClrType).Property<string>("ChangedBy");
            }
        }

        public override int SaveChanges()
        {
            auditEntities();
            return base.SaveChanges();
        }

        public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default(CancellationToken))
        {
            auditEntities();
            return base.SaveChangesAsync(cancellationToken);
        }

        private void auditEntities()
        {
            var uname = "System";
            if (_httpContextAccessor.HttpContext != null)
            {
                var user = _httpContextAccessor.HttpContext.User;
                if (user != null)
                {
                    var identity = user.Identity;
                    if (identity != null)
                    {
                        uname = user.FindFirstValue("preferred_username");
                        if (String.IsNullOrEmpty(uname)) uname = user.Identity.Name;
                    }
                }
            }

            foreach (EntityEntry<IAuditable> entry in ChangeTracker.Entries<IAuditable>())
            {
                if (entry.State == EntityState.Added)
                {
                    entry.Property("CreatedAt").CurrentValue = DateTime.Now;
                    entry.Property("CreatedBy").CurrentValue = uname;
                }
                else if (entry.State == EntityState.Modified)
                {
                    entry.Property("ChangedAt").CurrentValue = DateTime.Now;
                    entry.Property("ChangedBy").CurrentValue = uname;
                }
            }
        }
    }
}
