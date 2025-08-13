namespace Kite.Application.Models;

public class MessageModel
{
    public Guid Id { get; set; }
    public Guid ConversationId { get; set; }
    public string SenderId { get; set; } = string.Empty;
    public string SenderName { get; set; } = string.Empty;
    public string SenderProfilePictureUrl { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;
    public DateTimeOffset SentAt { get; set; }
    public string TimeElapsed { get; set; } = string.Empty;
    public bool IsRead { get; set; }
    public List<AttachedFileModel> Files { get; set; } = [];
}