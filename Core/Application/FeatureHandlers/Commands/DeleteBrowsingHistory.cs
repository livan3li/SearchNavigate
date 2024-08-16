using MediatR;
using SearchNavigate.Core.Application.Interfaces.Repositories;
using SearchNavigate.Request.Commands;
using Microsoft.EntityFrameworkCore;

namespace SearchNavigate.Core.Application.FeatureHandlers.Commands;

/// <summary>
/// 
/// </summary>
public class DeleteBrowsingHistoryCommandHandler : IRequestHandler<DeleteBrowsingHistoryCommand, bool>
{
    IUserViewHistoryRepository userViewHistoryRepository;

    public DeleteBrowsingHistoryCommandHandler(IUserViewHistoryRepository userViewHistoryRepository)
    {
        this.userViewHistoryRepository = userViewHistoryRepository;
    }

    /// <summary>
    /// Deletes browsing history(entry/row) for a given user and product.
    /// </summary>
    /// <param name="request">The DeleteBrowsingHistoryCommand containing the UserId and ProductId to delete.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>A boolean indicating whether the operation was successful.</returns>
    public async Task<bool> Handle(DeleteBrowsingHistoryCommand request, CancellationToken cancellationToken)
    {
        await userViewHistoryRepository.DeleteRangeAsync(i => i.UserId == request.UserId && i.ProductId == request.ProductId);
        return await Task.FromResult(true);
    }
}