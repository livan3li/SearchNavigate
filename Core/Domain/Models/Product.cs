namespace SearchNavigate.Core.Domain.Models;
public class Product : BaseEntity
{
   public string ProductName { get; set; }

   public Guid CategoryId { get; set; }

   public virtual Category Category { get; set; }
}
