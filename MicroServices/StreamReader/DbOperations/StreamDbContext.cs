using System;
using System.Reflection;
using Microsoft.EntityFrameworkCore;
using SearchNavigate.Core.Domain.Models;

namespace StreamReader.DbOperations;

public class StreamDbContext : DbContext
{
   public const string DefaultSchema = "DBO";

   public StreamDbContext(DbContextOptions options) : base(options)
   {

   }
   public DbSet<UserViewHistory> DbSetViewHistory { get; set; }

   protected override void OnModelCreating(ModelBuilder modelBuilder)
   {
      modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
   }
}
