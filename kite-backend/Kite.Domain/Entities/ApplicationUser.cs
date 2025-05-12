using Microsoft.AspNetCore.Identity;

namespace Kite.Domain.Entities;

public class ApplicationUser : IdentityUser
{
    public string FirstName { get; set; }
    public string LastName { get; set; }
    
    // Navigation property for friendships where this user is UserIdOne
    public ICollection<Friendship>? FriendshipsInitiated { get; set; }
    // Navigation property for friendships where this user is UserIdTwo
    public ICollection<Friendship>? FriendshipsReceived { get; set; }
    
    public string? ImageUrl { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}