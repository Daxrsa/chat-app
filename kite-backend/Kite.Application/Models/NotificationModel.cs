using Kite.Domain.Enums;

namespace Kite.Application.Models;

public class NotificationModel
{
    public Guid Id { get; set; }
    public string? Title { get; set; }
    public string Message { get; set; }
    public NotificationType Type { get; set; }
    public bool IsRead { get; set; }
    public DateTimeOffset CreatedAt { get; set; }
    public string TimeElapsed { get; set; }
    public DateTimeOffset? ReadAt { get; set; }
    public string ReceiverId { get; set; }
    public UserModel Receiver { get; set; }
}