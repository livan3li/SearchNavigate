namespace SearchNavigate.Core.Domain.Models;
public class UserViewHistory : BaseEntity
{
   public Guid UserId { get; set; }
   public Guid ProductId { get; set; }
   public DateTime ViewDate { get; set; }
   public ViewSourse ViewSourse { get; set; }

   public virtual User ViewedBy { get; set; }
   public virtual Product ViewedProduct { get; set; }
}

public enum ViewSourse
{
   Mobile = 1,
   Desktop = 2
}