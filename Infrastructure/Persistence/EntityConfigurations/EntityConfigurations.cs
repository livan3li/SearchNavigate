using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SearchNavigate.Core.Domain.Models;
using SearchNavigate.Infrastructure.Persistence.Context;

namespace SearchNavigate.Infrastructure.Persistence.EntityConfigurations;
public class UserEntityConfiguration : BaseEntityConfiguration<User>
{
   public override void Configure(EntityTypeBuilder<User> builder)
   {
      base.Configure(builder);
      builder.ToTable(nameof(User), SearchNavigateDbContext.DefaultSchema);
   }
}

public class ProductEntityConfiguration : BaseEntityConfiguration<Product>
{
   public override void Configure(EntityTypeBuilder<Product> builder)
   {
      base.Configure(builder);
      builder.ToTable(nameof(Core.Domain.Models.Product), SearchNavigateDbContext.DefaultSchema);

      builder
         .HasOne(p => p.Category)
         .WithMany()
         .HasForeignKey(i => i.CategoryId);
   }
}

public class CategoryEntityConfiguration : BaseEntityConfiguration<Category>
{
   public override void Configure(EntityTypeBuilder<Category> builder)
   {
      base.Configure(builder);
      builder.ToTable(nameof(Core.Domain.Models.Category), SearchNavigateDbContext.DefaultSchema);

   }
}

public class OrderEntityConfiguration : BaseEntityConfiguration<Order>
{
   public override void Configure(EntityTypeBuilder<Order> builder)
   {
      base.Configure(builder);
      builder.ToTable(nameof(Core.Domain.Models.Order), SearchNavigateDbContext.DefaultSchema);

      builder
         .HasOne(o => o.User)
         .WithMany(u => u.Orders)
         .HasForeignKey(i => i.OrderedBy);
   }
}

public class OrderItemsEntityConfiguration : BaseEntityConfiguration<OrderItems>
{
   public override void Configure(EntityTypeBuilder<OrderItems> builder)
   {
      base.Configure(builder);
      builder.ToTable(nameof(Core.Domain.Models.OrderItems), SearchNavigateDbContext.DefaultSchema);

      builder
         .HasOne(oi => oi.Order)
         .WithMany(o => o.Items)
         .HasForeignKey(i => i.OrderId);

      builder
         .HasOne(o => o.Product)
         .WithMany()
         .HasForeignKey(i => i.ProductId);
   }
}

public class ViewHistoryEntityConfiguration : BaseEntityConfiguration<UserViewHistory>
{
   public override void Configure(EntityTypeBuilder<UserViewHistory> builder)
   {
      base.Configure(builder);
      builder.ToTable(nameof(Core.Domain.Models.UserViewHistory), SearchNavigateDbContext.DefaultSchema);

      builder
         .HasOne(vh => vh.ViewedBy)
         .WithMany(wb => wb.ViewHistory)
         .HasForeignKey(i => i.UserId);

      builder
         .HasOne(vh => vh.ViewedProduct)
         .WithMany()
         .HasForeignKey(i => i.ProductId);

      builder
         .Property(i => i.ViewDate)
         .HasColumnType("TIMESTAMP")
         .ValueGeneratedOnAdd();
   }
}
