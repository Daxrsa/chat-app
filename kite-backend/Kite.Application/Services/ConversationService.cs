using Kite.Application.Interfaces;
using Kite.Application.Models;
using Kite.Domain.Common;
using Kite.Domain.Entities;
using Kite.Domain.Enums;
using Kite.Domain.Interfaces;
using Microsoft.AspNetCore.Identity;

namespace Kite.Application.Services;

public class ConversationService(
    IUserAccessor userAccessor,
    IConversationRepository conversationRepository,
    UserManager<ApplicationUser> userManager,
    IUnitOfWork unitOfWork,
    IApplicationFileRepository applicationFileRepository
) : IConversationService
{
    public async Task<Result<ConversationModel>> CreateConversationAsync(List<string> participantIds, CancellationToken cancellationToken = default)
    {
        var currentUserId = userAccessor.GetCurrentUserId();
        if (string.IsNullOrEmpty(currentUserId))
        {
            return Result<ConversationModel>.Failure(new Error("Auth.Unauthorized",
                "User must be authenticated."));
        }

        participantIds ??= [];
        if (!participantIds.Contains(currentUserId))
        {
            participantIds.Add(currentUserId);
        }

        var distinctParticipantIds = participantIds.Distinct().ToList();

        if (distinctParticipantIds.Count < 2)
        {
            return Result<ConversationModel>.Failure(new Error("Conversation.InvalidParticipants",
                "A conversation requires at least two participants."));
        }

        if (distinctParticipantIds.Count > 20)
        {
            return Result<ConversationModel>.Failure(new Error("Conversation.TooManyParticipants",
                "A conversation cannot have more than 20 participants."));
        }

        var existingConversation =
            await conversationRepository.FindByParticipantsAsync(distinctParticipantIds,
                cancellationToken);
        if (existingConversation != null)
        {
            return Result<ConversationModel>.Failure(new Error("Conversation.Exists",
                "A conversation with these participants already exists."));
        }

        var conversation = new Conversation
        {
            Id = Guid.NewGuid(),
            CreatedAt = DateTimeOffset.UtcNow,
            Participants = []
        };

        var participantModels = new List<ConversationParticipantModel>();

        foreach (var userId in distinctParticipantIds)
        {
            var user = await userManager.FindByIdAsync(userId);
            if (user == null) continue;

            conversation.Participants.Add(new ConversationParticipant
            {
                Id = Guid.NewGuid(),
                ConversationId = conversation.Id,
                UserId = userId,
                JoinedAt = DateTimeOffset.UtcNow
            });

            var authorProfilePicture =
                await applicationFileRepository.GetLatestUserFileByTypeAsync(userId,
                    FileType.Post, cancellationToken);

            participantModels.Add(new ConversationParticipantModel
            {
                UserId = user.Id,
                UserName = user.UserName ?? string.Empty,
                FirstName = user.FirstName,
                LastName = user.LastName,
                ProfilePictureUrl = authorProfilePicture?.FilePath ?? string.Empty,
                JoinedAt = DateTimeOffset.UtcNow
            });
        }

        if (conversation.Participants.Count < 2)
        {
            return Result<ConversationModel>.Failure(new Error("Conversation.InvalidParticipants",
                "A conversation requires at least two valid participants."));
        }

        if (conversation.Participants.Count == 2)
        {
            var otherUser = participantModels.First(p => p.UserId != currentUserId);
            conversation.Name = $"{otherUser.FirstName} {otherUser.LastName}".Trim();
        }

        await conversationRepository.InsertAsync(conversation, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        var conversationModel = new ConversationModel
        {
            Id = conversation.Id,
            Name = conversation.Name,
            CreatedAt = conversation.CreatedAt,
            Participants = participantModels,
            UnreadCount = 0
        };

        // Notify other participants via SignalR
        // var otherParticipantIds = distinctParticipantIds.Where(id => id != currentUserId);
        // await hubContext.Clients.Users(otherParticipantIds)
        //     .SendAsync("ConversationCreated", conversationModel, cancellationToken);

        return Result<ConversationModel>.Success(conversationModel);
    }

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

    public Task<Result<IEnumerable<MessageModel>>> GetConversationMessagesAsync(Guid conversationId,
        CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public Task<Result<bool>> MarkConversationAsReadAsync(Guid conversationId,
        CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }
}