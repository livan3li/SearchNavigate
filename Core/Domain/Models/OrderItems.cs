namespace SearchNavigate.Core.Domain.Models;

public class OrderItems : BaseEntity
{
   public Guid OrderId { get; set; }
   public Guid ProductId { get; set; }
   public int Quantity { get; set; } = 0;

   public virtual Product Product { get; set; }
   public virtual Order Order { get; set; }
}