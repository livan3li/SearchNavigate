namespace SearchNavigate.Core.Domain.Models;
public class User : BaseEntity
{
   public string UserName { get; set; }
   public string FirstName { get; set; }
   public string LastName { get; set; }

   public virtual ICollection<Order> Orders { get; set; }

   public virtual ICollection<UserViewHistory> ViewHistory { get; set; }
}