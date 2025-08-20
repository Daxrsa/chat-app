using AutoMapper;
using Kite.Application.Interfaces;
using Kite.Application.Models;
using Kite.Domain.Common;
using Kite.Domain.Enums;
using Kite.Domain.Interfaces;

namespace Kite.Application.Services;

public class UserService(
    IUserAccessor userAccessor,
    IConversationRepository conversationRepository,
    IApplicationFileRepository applicationFileRepository,
    IMapper mapper,
    IPostRepository postRepository) : IUserService
{
    public async Task<Result<IEnumerable<ConversationModel>>> GetUserConversationsAsync(
        CancellationToken cancellationToken = default)
    {
        var currentUserId = userAccessor.GetCurrentUserId();
        if (string.IsNullOrEmpty(currentUserId))
        {
            return Result<IEnumerable<ConversationModel>>.Failure(new Error("Auth.Unauthorized",
                "User must be authenticated."));
        }

        var conversations =
            await conversationRepository.GetConversationsByUserIdAsync(currentUserId,
                cancellationToken);
        var conversationModels = new List<ConversationModel>();

        foreach (var conversation in conversations)
        {
            var currentUserParticipant =
                conversation.Participants.First(p => p.UserId == currentUserId);

            var unreadCount = conversation.Messages
                .Count(m =>
                    m.SentAt > currentUserParticipant.LastReadAt &&
                    m.SenderId != currentUserId);

            var participantModels = new List<ConversationParticipantModel>();
            foreach (var participant in conversation.Participants)
            {
                var profilePicture =
                    await applicationFileRepository.GetLatestUserFileByTypeAsync(participant.UserId,
                        FileType.ProfilePicture, cancellationToken);
                participantModels.Add(new ConversationParticipantModel
                {
                    UserId = participant.UserId,
                    UserName = participant.User.UserName ?? string.Empty,
                    FirstName = participant.User.FirstName,
                    LastName = participant.User.LastName,
                    ProfilePictureUrl = profilePicture?.FilePath ?? string.Empty,
                    JoinedAt = participant.JoinedAt
                });
            }

            var conversationName = conversation.Name;
            if (string.IsNullOrEmpty(conversationName) && conversation.Participants.Count == 2)
            {
                var otherUser = participantModels.FirstOrDefault(p => p.UserId != currentUserId);
                if (otherUser != null)
                {
                    conversationName = $"{otherUser.FirstName} {otherUser.LastName}".Trim();
                }
            }

            conversationModels.Add(new ConversationModel
            {
                Id = conversation.Id,
                Name = conversationName,
                CreatedAt = conversation.CreatedAt,
                Participants = participantModels,
                UnreadCount = unreadCount
            });
        }

        return Result<IEnumerable<ConversationModel>>.Success(conversationModels);
    }

    public async Task<Result<List<PostModel>>> GetPostsForCurrentUserAsync(
        CancellationToken cancellationToken = default)
    {
        var currentUserId = userAccessor.GetCurrentUserId();
        if (string.IsNullOrEmpty(currentUserId))
        {
            return Result<List<PostModel>>.Failure(new Error("Auth.Unauthorized",
                "User must be authenticated to create posts"));
        }

        var posts = await postRepository.GetPostsForUserAsync(currentUserId, cancellationToken);
        if (posts is null)
        {
            return Result<List<PostModel>>.Failure(new Error("Posts.NotFound", "No posts found"));
        }

        var postModels = mapper.Map<List<PostModel>>(posts);

        return Result<List<PostModel>>.Success(postModels);
    }

    public async Task<Result<List<PostModel>>> GetPostsForUserAsync(string userId,
        CancellationToken cancellationToken = default)
    {
        var posts = await postRepository.GetPostsForUserAsync(userId, cancellationToken);
        if (posts is null)
        {
            return Result<List<PostModel>>.Failure(new Error("Posts.NotFound", "No posts found"));
        }

        var postModels = mapper.Map<List<PostModel>>(posts);

        return Result<List<PostModel>>.Success(postModels);
    }
}