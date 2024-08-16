using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SearchNavigate.Core.Domain.Models;

namespace SearchNavigate.Infrastructure.Persistence.EntityConfigurations;
public abstract class BaseEntityConfiguration<TEntity> : IEntityTypeConfiguration<TEntity> where TEntity : BaseEntity
{
   public virtual void Configure(EntityTypeBuilder<TEntity> builder)
   {
      builder.HasKey(i => i.Id);

      builder.Property(i => i.Id).ValueGeneratedOnAdd();
   }
}