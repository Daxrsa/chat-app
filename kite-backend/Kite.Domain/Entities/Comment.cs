using Kite.Domain.Interfaces;

namespace Kite.Domain.Entities;

public class Comment : IEntity<Guid>
{
    public Guid Id { get; set; }
    public string Content { get; set; } = string.Empty;
    public string UserId { get; set; } = string.Empty;
    public Guid PostId { get; set; } = Guid.Empty;
    public Guid? ParentCommentId { get; set; }
    public DateTimeOffset CreatedAt { get; set; }
    public DateTimeOffset? UpdatedAt { get; set; }
    public ICollection<ApplicationFile>? Files { get; set; }
    public Post Post { get; set; } = new();
    public ApplicationUser User { get; set; } = new();
    public Comment? ParentComment { get; set; }
    public ICollection<Comment> Replies { get; set; } = new List<Comment>();
}