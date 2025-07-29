using Kite.Domain.Interfaces;

namespace Kite.Domain.Entities;

public class Comment : IEntity<Guid>
{
    public Guid Id { get; set; }
    public string Content { get; set; }
    public string UserId { get; set; }
    public Guid PostId { get; set; }
    public Guid? ParentCommentId { get; set; }
    public DateTimeOffset CreatedAt { get; set; }
    public DateTimeOffset? UpdatedAt { get; set; }
    public Post Post { get; set; }
    public ApplicationUser User { get; set; }
    public Comment? ParentComment { get; set; }
    public ICollection<Comment> Replies { get; set; } = new List<Comment>();
}