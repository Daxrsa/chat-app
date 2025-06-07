using Kite.Domain.Interfaces;

namespace Kite.Domain.Entities;

public class Post : IEntity<Guid>
{
    public Guid Id { get; set; }
    public string Title { get; set; }
    public string Body { get; set; }
    public DateTimeOffset CreatedAt { get; set; }
    public string UserId { get; set; }
    public ApplicationUser User { get; set; }
}