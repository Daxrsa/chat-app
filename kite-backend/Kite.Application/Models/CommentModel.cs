namespace Kite.Application.Models;

public class CommentModel
{
    public Guid Id { get; set; }
    public string Content { get; set; } = string.Empty;
    public Guid? ParentCommentId { get; set; } 
    public DateTimeOffset CreatedAt { get; set; }
    public DateTimeOffset? UpdatedAt { get; set; }
    public string UserId { get; set; } = string.Empty;
    public string AuthorFirstName { get; set; } = string.Empty;
    public string AuthorLastName { get; set; } = string.Empty;
    public string AuthorUserName { get; set; } = string.Empty;
    public string? AuthorProfilePicture { get; set; }
    public int ReactionCount { get; set; }
    public int? ReplyCount { get; set; }
    public bool IsReactedByCurrentUser { get; set; }
    public List<string>? MentionedUsers { get; set; } = new();
    public List<AttachedFileModel> Files { get; set; } = new();
    public string TimeElapsed { get; set; } = string.Empty;
    public bool IsEdited { get; set; } 
}