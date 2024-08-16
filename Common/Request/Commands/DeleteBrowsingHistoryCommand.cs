using System;
using System.Threading.Tasks;
using MediatR;

namespace SearchNavigate.Request.Commands;

/// <summary>
/// 
/// </summary>
public class DeleteBrowsingHistoryCommand : IRequest<bool>
{
    public Guid UserId { get; set; }

    public Guid ProductId { get; set; }
}