using System.ComponentModel.DataAnnotations.Schema;
using Kite.Domain.Interfaces;
using Microsoft.AspNetCore.Identity;

namespace Kite.Domain.Entities;

public class ApplicationUser : IdentityUser, IEntity<string>
{
    public required string FirstName { get; set; }
    public required string LastName { get; set; }
    public ICollection<Reaction> Reactions { get; set; } = new List<Reaction>();
    public ICollection<FriendRequest>? FriendRequests { get; set; }
    public ICollection<Notification>? Notifications { get; set; }
    public ICollection<ApplicationFile>? Files { get; set; }
    public ICollection<Post>? Posts { get; set; }
    public DateTimeOffset CreatedAt { get; set; } 
}