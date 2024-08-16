using SearchNavigate.Common.Infrastructure.ResponseModel;

namespace SearchNavigate.Common.ViewModels;
public class BestSellerProductsViewModel
{
   public UserModel User { get; set; }
   public List<ProductModel> Products { get; set; }
   public string Type { get; set; }
}
