using System.ComponentModel.DataAnnotations.Schema;
using Kite.Domain.Interfaces;

namespace Kite.Domain.Entities;

public class Friendship : IEntity<Guid>
{
    public Guid Id { get; set; }
    public Guid FriendRequestId { get; set; }
    public required FriendRequest FriendRequest { get; set; }
    [NotMapped]
    public DateTimeOffset CreatedAt { get; set; } 
}