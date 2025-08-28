using Kite.Application.Interfaces;
using Kite.Application.Models;
using Kite.Domain.Common;

namespace Kite.Application.Services;

public class MessageService : IMessageService
{
    public async Task<Result<MessageModel>> SendMessageAsync(MessageModel message, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public async Task<Result<MessageModel>> EditMessageAsync(MessageModel message, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public async Task<Result<bool>> DeleteMessageAsync(Guid messageId, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public async Task<Result<int>> GetMessageTotalReactionsAsync(Guid messageId, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }
}