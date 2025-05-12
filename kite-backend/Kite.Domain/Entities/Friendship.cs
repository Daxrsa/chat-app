using Kite.Domain.Enums;

namespace Kite.Domain.Entities;

public class Friendship
{
    public string Id { get; set; }
    public string UserIdOne { get; set; }
    public string UserIdTwo { get; set; }
    public ApplicationUser UserOne { get; set; }
    public ApplicationUser UserTwo { get; set; }
    public FriendshipStatus Status { get; set; } = FriendshipStatus.Pending;
    public DateTime RequestSentTime { get; set; } = DateTime.UtcNow;
}