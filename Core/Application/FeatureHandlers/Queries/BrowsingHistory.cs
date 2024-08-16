using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using SearchNavigate.Common.Infrastructure.ResponseModel;
using SearchNavigate.Common.ViewModels;
using SearchNavigate.Core.Application.Interfaces.Repositories;
using SearchNavigate.Request.Queries;

namespace SearchNavigate.Core.Application.FeatureHandlers.Queries;

public class BrowsingHistoryQuerydHandler : IRequestHandler<BrowsingHistoryQuery, BrowsingHistoryViewModel>
{
   private readonly IUserRepository userRepository;

   public BrowsingHistoryQuerydHandler(IUserRepository userRepository)
   {
      this.userRepository = userRepository;
   }

   /// <summary>
   /// Gets browsing history of user. 
   /// </summary>
   /// <param name="request">A BrowsingHistoryQuery containing the UserId</param>
   /// <param name="cancellationToken">A token to cancel the operation.</param>
   /// <returns>A BrowsingHistoryViewModel representing the user's browsing history.
   /// If browsing history is less than 5, then empty list is returned instead.</returns>
   public async Task<BrowsingHistoryViewModel> Handle(BrowsingHistoryQuery request, CancellationToken cancellationToken)
   {
      var query = userRepository.AsQueryable();

      if (request.UserId != Guid.Empty)
      {
         query = query.Where(i => i.Id == request.UserId);
         query = query.Include(i => i.ViewHistory);
         query = query.OrderByDescending(i => i.ViewHistory.FirstOrDefault().ViewDate);
         var selectedResult = query.Select(i => new BrowsingHistoryViewModel()
         {
            User = new UserModel()
            {
               Id = i.Id,
               UserName = i.UserName
            },
            Products = i.ViewHistory
               .Select(i => new ProductModel()
               {
                  ProductId = i.ProductId,
                  CategoryName = i.ViewedProduct.Category.CategoryName,
                  ProductName = i.ViewedProduct.ProductName
               })
               .Take(10)
               .ToList(),
            Type = "Personalized"
         });


         BrowsingHistoryViewModel result = await selectedResult.FirstOrDefaultAsync();
         result.Products = result.Products.Count < 5 ?
            new List<ProductModel>() :
            result.Products; ;

         return result;
      }
      else
         return null;
   }
}
