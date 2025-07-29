using Kite.Domain.Enums;

namespace Kite.Application.Models;

public class ReactionModel
{
    public Guid Id { get; set; }
    public string Firstname { get; set; } = string.Empty;
    public string Lastname { get; set; } = string.Empty;
    public ReactionType ReactionType { get; set; }
    public string ProfilePicture { get; set; } = string.Empty;
    public DateTimeOffset CreatedAt { get; set; }
}