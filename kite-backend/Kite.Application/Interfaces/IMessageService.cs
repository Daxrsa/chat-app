using Kite.Application.Models;
using Kite.Domain.Common;

namespace Kite.Application.Interfaces;

public interface IMessageService
{
    Task<Result<MessageModel>> SendMessageAsync(MessageModel message, CancellationToken cancellationToken = default);
    Task<Result<MessageModel>> EditMessageAsync(MessageModel message, CancellationToken cancellationToken = default);
    Task<Result<bool>> DeleteMessageAsync(Guid messageId, CancellationToken cancellationToken = default);
}

