using Kite.Domain.Enums;
using Kite.Domain.Interfaces;

namespace Kite.Domain.Entities;

public class Friendship : IEntity<Guid>
{
    public Guid Id { get; set; }
    public string SenderId { get; set; }
    public string ReceiverId { get; set; }
    public ApplicationUser UserOne { get; set; }
    public ApplicationUser UserTwo { get; set; }
    public FriendRequestStatus Status { get; set; }
    public DateTime RequestSentTime { get; set; }
    public DateTime RequestReceivedTime { get; set; }
    public DateTime ResendRequestTime { get; set; }
}