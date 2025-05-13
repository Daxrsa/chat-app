using Kite.Domain.Enums;

namespace Kite.Application.Models;

public class FriendRequestModel
{
    public Guid Id { get; set; }
    public string SenderId { get; set; }
    public string SenderUsername { get; set; }
    public string SenderFirstName { get; set; }
    public string SenderLastName { get; set; }
    public string SenderImageUrl { get; set; }
    public string ReceiverId { get; set; }
    public string ReceiverUsername { get; set; }
    public string ReceiverFirstName { get; set; }
    public string ReceiverLastName { get; set; }
    public string ReceiverImageUrl { get; set; }
    public DateTime CreatedAt { get; set; }
    public string TimeElapsed { get; set; }
    public FriendRequestStatus Status { get; set; }
}