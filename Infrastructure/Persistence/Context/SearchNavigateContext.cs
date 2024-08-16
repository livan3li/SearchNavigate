using System.Reflection;
using Microsoft.EntityFrameworkCore;
using SearchNavigate.Core.Domain.Models;

namespace SearchNavigate.Infrastructure.Persistence.Context;
public class SearchNavigateDbContext : DbContext
{
   public const string DefaultSchema = "DBO";

   public SearchNavigateDbContext(DbContextOptions options) : base(options)
   {
   }
   public DbSet<Category> DbSetCategory { get; set; }
   public DbSet<Order> DbSetOrder { get; set; }
   public DbSet<OrderItems> DbSetOrderItems { get; set; }
   public DbSet<Product> DbSetProduct { get; set; }
   public DbSet<User> DbSetUser { get; set; }
   public DbSet<UserViewHistory> DbSetViewHistory { get; set; }

   protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
   {
      if (!optionsBuilder.IsConfigured)
      {
         optionsBuilder.UseNpgsql(
            "Host=localhost; Port=5432; Database=SearchNavigate;User Id=postgres;Password=132652;",
            opt =>
            {
               opt.EnableRetryOnFailure();
            });
      }
   }

   protected override void OnModelCreating(ModelBuilder modelBuilder)
   {
      // OnBeforeSave();
      // modelBuilder.ForNpgsqlUseIdentityColumns();
      modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
   }

   // public override int SaveChanges()
   // {
   //    OnBeforeSave();
   //    return base.SaveChanges();
   // }

   // public override int SaveChanges(bool acceptAllChangesOnSuccess)
   // {
   //    OnBeforeSave();
   //    return base.SaveChanges(acceptAllChangesOnSuccess);
   // }
   // public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
   // {
   //    OnBeforeSave();
   //    return base.SaveChangesAsync(cancellationToken);
   // }

   // public override Task<int> SaveChangesAsync(bool acceptAllChangesOnSuccess, CancellationToken cancellationToken = default)
   // {
   //    OnBeforeSave();
   //    return base.SaveChangesAsync(acceptAllChangesOnSuccess, cancellationToken);
   // }

   // private void OnBeforeSave()
   // {
   //    var addedEntities = ChangeTracker.Entries()
   //                            .Where(i => i.State == EntityState.Added)
   //                            .Select(i => (BaseEntity)i.Entity);
   // }
}
