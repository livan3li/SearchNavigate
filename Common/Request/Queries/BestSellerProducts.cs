using MediatR;
using SearchNavigate.Common.ViewModels;

namespace SearchNavigate.Request.Queries;
public class BestSellerProductsQuery : IRequest<BestSellerProductsViewModel>
{
   public Guid UserId { get; set; }

   public BestSellerProductsQuery(Guid userId)
   {
      UserId = userId;
   }
}
