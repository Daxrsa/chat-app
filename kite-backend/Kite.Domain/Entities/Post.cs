using Kite.Domain.Enums;
using Kite.Domain.Interfaces;

namespace Kite.Domain.Entities;

public class Post : IEntity<Guid>
{
    public Guid Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Body { get; set; } = string.Empty;
    public int ReactionCount { get; set; }
    public ICollection<Reaction> Reactions { get; set; } = new List<Reaction>();
    public ICollection<Comment> Comments { get; set; } = new List<Comment>();
    public int CommentCount { get; set; }
    public int ShareCount { get; set; }
    public PostVisibility Visibility { get; set; } = PostVisibility.Public;
    public string UserId { get; set; } = string.Empty;
    public ApplicationUser User { get; set; } = new();
    public List<ApplicationUser>? MentionedUsers { get; set; } = new();
    public List<string>? Hashtags { get; set; } = new();
    public ICollection<ApplicationFile>? Files { get; set; }
    public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;
    public DateTimeOffset? UpdatedAt { get; set; } = DateTimeOffset.UtcNow;
    public bool IsEdited { get; set; }
    public bool IsHidden { get; set; } 
}