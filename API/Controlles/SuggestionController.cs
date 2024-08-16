using MediatR;
using Microsoft.AspNetCore.Mvc;
using SearchNavigate.Common.ViewModels;
using SearchNavigate.Request.Commands;
using SearchNavigate.Request.Queries;

namespace SearchNavigate.API.Controllers;

[Route("API/[controller]/[action]")]
[ApiController]
public class SuggestionController : ControllerBase
{
   private readonly IMediator mediator;

   public SuggestionController(IMediator mediator)
   {
      this.mediator = mediator;
   }

   [HttpGet]
   public async Task<ActionResult<BrowsingHistoryViewModel>> GetBrowsingHistory(Guid userId)
   {
      var browsingHistory = await mediator.Send(new BrowsingHistoryQuery(userId));
      return Ok(browsingHistory);
   }

   [HttpPost]
   public async Task<ActionResult<BrowsingHistoryViewModel>> DeleteBrowsingHistory(Guid userId, Guid productId)
   {
      var deletedHistory = await mediator.Send(
         new DeleteBrowsingHistoryCommand()
         {
            UserId = userId,
            ProductId = productId
         });
      return Ok(deletedHistory);
   }

   [HttpGet]
   public async Task<ActionResult<BestSellerProductsViewModel>> GetSuggestionFromBestSeller(Guid userId)
   {
      var browsingHistory = await mediator.Send(new BestSellerProductsQuery(userId));
      return Ok(browsingHistory);
   }
}
