using Kite.Domain.Enums;
using Kite.Domain.Interfaces;

namespace Kite.Domain.Entities;

public class Post : IEntity<Guid>
{
    public Guid Id { get; set; }
    public string Title { get; set; }
    public string Body { get; set; }
    public int ReactionCount { get; set; }
    public ICollection<Reaction> Reactions { get; set; } = new List<Reaction>();
    public int CommentCount { get; set; }
    public int ShareCount { get; set; }
    public PostVisibility Visibility { get; set; } = PostVisibility.Public;
    public string UserId { get; set; }
    public string TimeElapsed { get; set; } = string.Empty;
    public ApplicationUser User { get; set; }
    public List<ApplicationUser>? MentionedUsers { get; set; } = new();
    public List<string>? Hashtags { get; set; } = new();
    public ICollection<ApplicationFile>? Files { get; set; }
    public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;
    public DateTimeOffset? UpdatedAt { get; set; } = DateTimeOffset.UtcNow;
    public bool IsEdited { get; set; }
    public bool IsHidden { get; set; } 
}