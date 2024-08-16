using MediatR;
using SearchNavigate.Common.ViewModels;

namespace SearchNavigate.Request.Queries;
public class BrowsingHistoryQuery : IRequest<BrowsingHistoryViewModel>
{
   public Guid UserId { get; set; }

   public BrowsingHistoryQuery(Guid userId)
   {
      UserId = userId;
   }
}
