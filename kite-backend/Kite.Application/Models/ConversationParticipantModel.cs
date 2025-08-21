using Kite.Domain.Enums;

namespace Kite.Application.Models;

public class ConversationParticipantModel
{
    public string UserId { get; set; } = string.Empty;
    public string UserName { get; set; } = string.Empty;
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public FriendRequestStatus FriendshipStatus { get; set; }
    public IEnumerable<UserModel>? MutualFriends { get; set; }
    public int? MutualFriendsCount { get; set; } 
    public IEnumerable<ConversationModel>? MutualConversations { get; set; }
    public int? MutualConversationsCount { get; set; } 
    public string ProfilePictureUrl { get; set; } = string.Empty;
    public DateTimeOffset JoinedAt { get; set; }
    public DateTimeOffset? LastReadAt { get; set; }
}