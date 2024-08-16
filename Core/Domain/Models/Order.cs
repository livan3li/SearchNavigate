namespace SearchNavigate.Core.Domain.Models;

public class Order : BaseEntity
{
   public Guid OrderedBy { get; set; }

   public virtual User User { get; set; }
   public virtual List<OrderItems> Items { get; set; }
}