using Kite.Application.Utilities;
using Kite.Domain.Enums;

namespace Kite.Application.Models;

public class PostModel
{
    public Guid Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Body { get; set; } = string.Empty;
    public DateTimeOffset CreatedAt { get; set; }
    public DateTimeOffset? UpdatedAt { get; set; }
    public string UserId { get; set; } = string.Empty;
    public string AuthorFirstName { get; set; } = string.Empty;
    public string AuthorLastName { get; set; } = string.Empty;
    public string AuthorUserName { get; set; } = string.Empty;
    public string? AuthorProfilePicture { get; set; }
    public int LikeCount { get; set; }
    public int CommentCount { get; set; }
    public int ShareCount { get; set; }
    public bool IsLikedByCurrentUser { get; set; }
    public List<string>? Hashtags { get; set; } = new();
    public List<string>? MentionedUsers { get; set; } = new();
    public List<AttachedFileModel> Files { get; set; } = new();
    public string TimeElapsed { get; set; } = string.Empty;
    public PostVisibility Visibility { get; set; } = PostVisibility.Public;
    public bool IsEdited { get; set; } 
    public bool IsHidden { get; set; } 
}