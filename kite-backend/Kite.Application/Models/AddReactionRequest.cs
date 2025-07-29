using Kite.Domain.Enums;

namespace Kite.Application.Models;

public class AddReactionRequest
{
    public EntityType EntityType { get; set; }
    public Guid EntityId { get; set; }
    public ReactionType ReactionType { get; set; }
}