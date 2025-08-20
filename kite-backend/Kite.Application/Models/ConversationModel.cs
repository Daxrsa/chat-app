namespace Kite.Application.Models;

public class ConversationModel
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public DateTimeOffset CreatedAt { get; set; }
    public string TimeElapsed { get; set; } = string.Empty;
    public MessageModel? LastMessage { get; set; }
    public int UnreadCount { get; set; }
    public List<ConversationParticipantModel> Moderators { get; set; } = [];
    public List<ConversationParticipantModel> Participants { get; set; } = [];
}