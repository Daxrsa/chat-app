using System.ComponentModel.DataAnnotations.Schema;
using Kite.Domain.Enums;
using Kite.Domain.Interfaces;

namespace Kite.Domain.Entities;

public class FriendRequest : IEntity<Guid>
{
    public Guid Id { get; set; }
    public required string SenderId { get; set; }
    public required string ReceiverId { get; set; }
    public ApplicationUser UserOne { get; set; } 
    public ApplicationUser UserTwo { get; set; }
    public FriendRequestStatus Status { get; set; }
    [NotMapped]
    public DateTimeOffset ResendRequestTime { get; set; } 
    [NotMapped]
    public DateTimeOffset CreatedAt { get; set; } 
    [NotMapped]
    public DateTimeOffset UpdatedAt { get; set; } 
    public Friendship? Friendship { get; set; }
}