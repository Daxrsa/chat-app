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
    INotificationService notificationService,
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
            OwnerId = currentUserId,
            Moderators = [],
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
        
        foreach (var participantId in distinctParticipantIds.Where(id => id != currentUserId))
        {
            var notificationModel = new NotificationModel
            {
                SenderId = currentUserId,
                ReceiverId = participantId,
                Message = $"You have been added to a conversation by {userAccessor.GetCurrentUserFirstName()} {userAccessor.GetCurrentUserLastName()}",
                Type = NotificationType.Conversation,
                IsRead = false,
                CreatedAt = DateTimeOffset.UtcNow
            };

            await notificationService.CreateNotificationAsync(notificationModel, cancellationToken);
        }

        return Result<ConversationModel>.Success(conversationModel);
    }

    public async Task<Result<IEnumerable<MessageModel>>> GetConversationMessagesAsync(Guid conversationId,
        CancellationToken cancellationToken = default)
    {
        var currentUserId = userAccessor.GetCurrentUserId();
        if (string.IsNullOrEmpty(currentUserId))
        {
            return Result<IEnumerable<MessageModel>>.Failure(new Error("Auth.Unauthorized",
                "User must be authenticated."));
        }

        var conversation = await conversationRepository.GetByIdAsync(conversationId, cancellationToken);
        if (conversation == null)
        {
            return Result<IEnumerable<MessageModel>>.Failure(new Error("Conversation.NotFound",
                "Conversation not found."));
        }
        
        var isParticipant = conversation.Participants.Any(p => p.UserId == currentUserId);
        if (!isParticipant)
        {
            return Result<IEnumerable<MessageModel>>.Failure(new Error("Conversation.Unauthorized",
                "User is not a participant in this conversation."));
        }

        var messageModels = new List<MessageModel>();
        foreach (var message in conversation.Messages.OrderBy(m => m.SentAt))
        {
            var sender = conversation.Participants.First(p => p.UserId == message.SenderId);
            var senderProfilePicture = await applicationFileRepository.GetLatestUserFileByTypeAsync(
                message.SenderId, FileType.ProfilePicture, cancellationToken);

            messageModels.Add(new MessageModel
            {
                Id = message.Id,
                ConversationId = message.ConversationId,
                SenderId = message.SenderId,
                SenderName = $"{sender.User.FirstName} {sender.User.LastName}".Trim(),
                SenderProfilePictureUrl = senderProfilePicture?.FilePath ?? string.Empty,
                Content = message.Content,
                SentAt = message.SentAt
            });
        }

        return Result<IEnumerable<MessageModel>>.Success(messageModels);
    }
    
    public async Task<Result<bool>> DeleteConversationAsync(Guid conversationId, CancellationToken cancellationToken = default)
    {
        var currentUserId = userAccessor.GetCurrentUserId();
        if (string.IsNullOrEmpty(currentUserId))
        {
            return Result<bool>.Failure(new Error("Auth.Unauthorized", "User must be authenticated."));
        }

        var conversation = await conversationRepository.GetByIdAsync(conversationId, cancellationToken);
        if (conversation is null)
        {
            return Result<bool>.Failure(new Error("Conversation.NotFound", "Conversation not found."));
        }

        if (conversation.OwnerId != currentUserId)
        {
            return Result<bool>.Failure(new Error("Conversation.Forbidden", "Only the owner can delete this conversation."));
        }

        await conversationRepository.DeleteAsync(conversation, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result<bool>.Success();
    }
}