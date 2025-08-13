using Kite.Application.Interfaces;
using Microsoft.AspNetCore.SignalR;

namespace Kite.Infrastructure.Hubs;

public class ChatHub(IUserAccessor userAccessor) : Hub
{
    public override async Task OnConnectedAsync()
    {
        var userId = userAccessor.GetCurrentUserId();
        if (!string.IsNullOrEmpty(userId))
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, userId);
        }

        await base.OnConnectedAsync();
    }

    public async Task JoinConversation(string conversationId)
    {
        await Groups.AddToGroupAsync(Context.ConnectionId, conversationId);
    }

    public async Task LeaveConversation(string conversationId)
    {
        await Groups.RemoveFromGroupAsync(Context.ConnectionId, conversationId);
    }
}