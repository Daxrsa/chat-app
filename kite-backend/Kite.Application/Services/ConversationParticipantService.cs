using Kite.Application.Interfaces;
using Kite.Application.Models;
using Kite.Domain.Common;
using Kite.Domain.Entities;
using Kite.Domain.Enums;
using Kite.Domain.Interfaces;
using Microsoft.AspNetCore.Identity;

namespace Kite.Application.Services;

public class ConversationParticipantService(
    IUserAccessor userAccessor,
    IConversationRepository conversationRepository,
    IConversationParticipantRepository conversationParticipantRepository,
    UserManager<ApplicationUser> userManager,
    IUnitOfWork unitOfWork,
    IFriendRequestRepository friendRequestRepository,
    IFriendshipService friendshipService,
    IUserService userService,
    IApplicationFileRepository applicationFileRepository) : IConversationParticipantService
{
    public async Task<Result<ConversationParticipantModel>> GetSingleParticipantAsync(
        Guid participantId,
        CancellationToken cancellationToken = default)
    {
        if (participantId == Guid.Empty)
            return Result<ConversationParticipantModel>.Failure(
                new Error("Participant.InvalidInput", "Participant id is required."));

        var currentUserId = userAccessor.GetCurrentUserId();
        if (string.IsNullOrWhiteSpace(currentUserId))
            return Result<ConversationParticipantModel>.Failure(
                new Error("Auth.Unauthorized", "User must be authenticated."));
        
        var participant =
            await conversationParticipantRepository.GetByIdAsync(participantId, cancellationToken);
        if (participant is null)
            return Result<ConversationParticipantModel>.Failure(
                new Error("Participant.NotFound", "Participant not found."));
        
        var conversation =
            await conversationRepository.GetByIdAsync(participant.ConversationId,
                cancellationToken);
        if (conversation is null)
            return Result<ConversationParticipantModel>.Failure(
                new Error("Conversation.NotFound", "Conversation not found."));

        var callerIsParticipant = conversation.Participants.Any(p => p.UserId == currentUserId);
        if (!callerIsParticipant)
            return Result<ConversationParticipantModel>.Failure(
                new Error("Auth.Forbidden", "You are not a participant of this conversation."));
        
        var user = await userManager.FindByIdAsync(participant.UserId);
        if (user is null)
            return Result<ConversationParticipantModel>.Failure(
                new Error("User.NotFound", "User associated with this participant not found."));
        
        var friendRequest = await friendRequestRepository.GetFriendRequestBetweenUsersAsync(
            currentUserId, user.Id, cancellationToken);

        var friendshipStatus = friendRequest is null ? FriendRequestStatus.None : friendRequest.Status;     

        var profilePicture = await applicationFileRepository.GetLatestUserFileByTypeAsync(
            user.Id, FileType.ProfilePicture, cancellationToken);
        
        var mutualFriends = await friendshipService.GetMutualFriendsAsync(
            user.Id, cancellationToken);
        
        var mutualConversations = await userService.GetMutualConversationsAsync(user.Id, cancellationToken);
        
        var model = new ConversationParticipantModel
        {
            UserId = user.Id,
            UserName = user.UserName ?? string.Empty,
            FirstName = user.FirstName,
            LastName = user.LastName,
            ProfilePictureUrl = profilePicture?.FilePath ?? string.Empty,
            JoinedAt = participant.JoinedAt,
            MutualFriends = mutualFriends.Value,
            MutualFriendsCount = mutualFriends?.Value?.Count(),
            MutualConversations = mutualConversations.Value, 
            MutualConversationsCount = mutualConversations?.Value?.Count(), 
            FriendshipStatus = friendshipStatus
        };

        return Result<ConversationParticipantModel>.Success(model);
    }

    public async Task<Result<ConversationParticipantModel>> AddParticipantAsync(Guid conversationId,
        string userId,
        CancellationToken cancellationToken = default)
    {
        var currentUserId = userAccessor.GetCurrentUserId();
        if (string.IsNullOrEmpty(currentUserId))
            return Result<ConversationParticipantModel>.Failure(new Error("Auth.Unauthorized",
                "User must be authenticated."));

        var conversation =
            await conversationRepository.GetByIdAsync(conversationId, cancellationToken);
        if (conversation == null)
            return Result<ConversationParticipantModel>.Failure(new Error("Conversation.NotFound",
                "The specified conversation does not exist."));

        if (conversation.Participants.All(p => p.UserId != currentUserId))
            return Result<ConversationParticipantModel>.Failure(new Error("Auth.Forbidden",
                "You are not a participant of this conversation."));

        if (conversation.Participants.Any(p => p.UserId == userId))
            return Result<ConversationParticipantModel>.Failure(new Error("Participant.Exists",
                "This user is already a participant in the conversation."));

        if (conversation.Participants.Count >= 10)
            return Result<ConversationParticipantModel>.Failure(new Error(
                "Conversation.ParticipantLimitReached",
                "The conversation has reached its maximum number of participants."));

        var userToAdd = await userManager.FindByIdAsync(userId);
        if (userToAdd == null)
        {
            return Result<ConversationParticipantModel>.Failure(new Error("User.NotFound",
                "The user you are trying to add does not exist."));
        }

        var newParticipant = new ConversationParticipant
        {
            Id = Guid.NewGuid(),
            ConversationId = conversationId,
            UserId = userId,
            JoinedAt = DateTimeOffset.UtcNow
        };

        conversation.Participants.Add(newParticipant);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        var profilePicture =
            await applicationFileRepository.GetLatestUserFileByTypeAsync(userId,
                FileType.ProfilePicture, cancellationToken);

        var participantModel = new ConversationParticipantModel
        {
            UserId = userToAdd.Id,
            UserName = userToAdd.UserName ?? string.Empty,
            FirstName = userToAdd.FirstName,
            LastName = userToAdd.LastName,
            ProfilePictureUrl = profilePicture?.FilePath ?? string.Empty,
            JoinedAt = newParticipant.JoinedAt
        };

        return Result<ConversationParticipantModel>.Success(participantModel);
    }

    public async Task<Result<bool>> RemoveParticipantAsync(Guid conversationId, string userId,
        CancellationToken cancellationToken = default)
    {
        var currentUserId = userAccessor.GetCurrentUserId();
        if (string.IsNullOrEmpty(currentUserId))
            return Result<bool>.Failure(new Error("Auth.Unauthorized",
                "User must be authenticated."));

        var conversation =
            await conversationRepository.GetByIdAsync(conversationId, cancellationToken);

        if (conversation == null)
            return Result<bool>.Failure(new Error("Conversation.NotFound",
                "The specified conversation does not exist."));

        if (conversation.Participants.All(p => p.UserId != currentUserId))
            return Result<bool>.Failure(new Error("Auth.Forbidden",
                "You are not a participant of this conversation."));

        var participantToRemove = conversation.Participants.FirstOrDefault(p => p.UserId == userId);
        if (participantToRemove == null)
            return Result<bool>.Failure(new Error("Participant.NotFound",
                "The specified user is not a participant in this conversation."));

        conversation.Participants.Remove(participantToRemove);
        await unitOfWork.SaveChangesAsync(cancellationToken);
        return Result<bool>.Success(true);
    }

    public async Task<Result<IEnumerable<ConversationParticipantModel>>> GetNonParticipantsAsync(
        Guid conversationId, CancellationToken cancellationToken = default)
    {
        var currentUserId = userAccessor.GetCurrentUserId();
        if (string.IsNullOrEmpty(currentUserId))
            return Result<IEnumerable<ConversationParticipantModel>>.Failure(new Error(
                "Auth.Unauthorized",
                "User must be authenticated."));

        var conversation =
            await conversationRepository.GetByIdAsync(conversationId, cancellationToken);
        if (conversation == null)
            return Result<IEnumerable<ConversationParticipantModel>>.Failure(new Error(
                "Conversation.NotFound",
                "The specified conversation does not exist."));

        if (conversation.Participants.All(p => p.UserId != currentUserId))
            return Result<IEnumerable<ConversationParticipantModel>>.Failure(new Error(
                "Auth.Forbidden",
                "You are not a participant of this conversation."));

        var nonParticipantUsers =
            await conversationParticipantRepository.GetNonParticipantsAsync(conversationId,
                cancellationToken);

        var nonParticipantModels = new List<ConversationParticipantModel>();
        foreach (var user in nonParticipantUsers)
        {
            var profilePicture =
                await applicationFileRepository.GetLatestUserFileByTypeAsync(user.Id,
                    FileType.ProfilePicture,
                    cancellationToken);

            nonParticipantModels.Add(new ConversationParticipantModel
            {
                UserId = user.Id,
                UserName = user.UserName ?? string.Empty,
                FirstName = user.FirstName,
                LastName = user.LastName,
                ProfilePictureUrl = profilePicture?.FilePath ?? string.Empty
            });
        }

        return Result<IEnumerable<ConversationParticipantModel>>.Success(nonParticipantModels);
    }

    public async Task<Result<List<ConversationParticipantModel>>> GetParticipantsAsync(
        Guid conversationId, CancellationToken cancellationToken = default)
    {
        var currentUserId = userAccessor.GetCurrentUserId();
        if (string.IsNullOrEmpty(currentUserId))
            return Result<List<ConversationParticipantModel>>.Failure(new Error("Auth.Unauthorized",
                "User must be authenticated."));

        var conversation =
            await conversationRepository.GetByIdAsync(conversationId, cancellationToken);
        if (conversation == null)
            return Result<List<ConversationParticipantModel>>.Failure(new Error(
                "Conversation.NotFound",
                "The specified conversation does not exist."));

        if (conversation.Participants.All(p => p.UserId != currentUserId))
            return Result<List<ConversationParticipantModel>>.Failure(new Error("Auth.Forbidden",
                "You are not a participant of this conversation."));

        var participantModels = new List<ConversationParticipantModel>();
        foreach (var participant in conversation.Participants)
        {
            var user = await userManager.FindByIdAsync(participant.UserId);
            if (user != null)
            {
                var profilePicture = await applicationFileRepository.GetLatestUserFileByTypeAsync(
                    user.Id, FileType.ProfilePicture, cancellationToken);

                participantModels.Add(new ConversationParticipantModel
                {
                    UserId = user.Id,
                    UserName = user.UserName ?? string.Empty,
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    ProfilePictureUrl = profilePicture?.FilePath ?? string.Empty,
                    JoinedAt = participant.JoinedAt
                });
            }
        }

        return Result<List<ConversationParticipantModel>>.Success(participantModels);
    }

    public async Task<Result<bool>> TransferOwnershipAsync(Guid conversationId,
        string newOwnerUserId,
        CancellationToken cancellationToken = default)
    {
        var currentUserId = userAccessor.GetCurrentUserId();
        if (string.IsNullOrWhiteSpace(currentUserId))
            return Result<bool>.Failure(new Error("Auth.Unauthorized",
                "User must be authenticated."));

        if (conversationId == Guid.Empty || string.IsNullOrWhiteSpace(newOwnerUserId))
            return Result<bool>.Failure(new Error("Conversation.InvalidInput",
                "Conversation and user are required."));

        var conversation =
            await conversationRepository.GetByIdAsync(conversationId, cancellationToken);
        if (conversation is null)
            return Result<bool>.Failure(new Error("Conversation.NotFound",
                "Conversation not found."));

        if (conversation.OwnerId != currentUserId)
            return Result<bool>.Failure(new Error("Conversation.Forbidden",
                "Only the owner can transfer ownership."));

        if (newOwnerUserId == currentUserId)
            return Result<bool>.Failure(new Error("Conversation.InvalidInput",
                "You are already the owner of this conversation."));

        var isParticipant = conversation.Participants.Any(p => p.UserId == newOwnerUserId);
        if (!isParticipant)
            return Result<bool>.Failure(new Error("Participant.NotFound",
                "New owner must be a participant."));

        conversation.OwnerId = newOwnerUserId;

        await unitOfWork.SaveChangesAsync(cancellationToken);
        return Result<bool>.Success(true);
    }

    public async Task<Result<bool>> PromoteToModeratorAsync(Guid conversationId, string userId,
        CancellationToken cancellationToken = default)
    {
        var currentUserId = userAccessor.GetCurrentUserId();
        if (string.IsNullOrWhiteSpace(currentUserId))
            return Result<bool>.Failure(new Error("Auth.Unauthorized",
                "User must be authenticated."));

        if (conversationId == Guid.Empty || string.IsNullOrWhiteSpace(userId))
            return Result<bool>.Failure(new Error("Conversation.InvalidInput",
                "Conversation and user are required."));

        var conversation =
            await conversationRepository.GetByIdAsync(conversationId, cancellationToken);
        if (conversation is null)
            return Result<bool>.Failure(new Error("Conversation.NotFound",
                "Conversation not found."));

        if (conversation.OwnerId != currentUserId)
            return Result<bool>.Failure(new Error("Conversation.Forbidden",
                "Only the owner can promote admins."));

        if (userId == conversation.OwnerId)
            return Result<bool>.Failure(new Error("Conversation.InvalidOperation",
                "Owner cannot be promoted."));

        var participant = conversation.Participants.FirstOrDefault(p => p.UserId == userId);
        if (participant is null)
            return Result<bool>.Failure(new Error("Participant.NotFound",
                "User is not a participant of this conversation."));

        if (participant.IsModerator)
            return Result<bool>.Success();

        participant.IsModerator = true;
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result<bool>.Success();
    }

    public async Task<Result<bool>> DemoteModeratorAsync(Guid conversationId, string userId,
        CancellationToken cancellationToken = default)
    {
        var currentUserId = userAccessor.GetCurrentUserId();
        if (string.IsNullOrWhiteSpace(currentUserId))
            return Result<bool>.Failure(new Error("Auth.Unauthorized",
                "User must be authenticated."));

        if (conversationId == Guid.Empty || string.IsNullOrWhiteSpace(userId))
            return Result<bool>.Failure(new Error("Conversation.InvalidInput",
                "Conversation and user are required."));

        var conversation =
            await conversationRepository.GetByIdAsync(conversationId, cancellationToken);
        if (conversation is null)
            return Result<bool>.Failure(new Error("Conversation.NotFound",
                "Conversation not found."));

        if (conversation.OwnerId != currentUserId)
            return Result<bool>.Failure(new Error("Conversation.Forbidden",
                "Only the owner can demote moderators."));

        if (userId == conversation.OwnerId)
            return Result<bool>.Failure(new Error("Conversation.InvalidOperation",
                "Owner cannot be demoted."));

        var participant = conversation.Participants.FirstOrDefault(p => p.UserId == userId);
        if (participant is null)
            return Result<bool>.Failure(new Error("Participant.NotFound",
                "User is not a participant of this conversation."));

        if (!participant.IsModerator)
            return Result<bool>.Success();

        participant.IsModerator = false;
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result<bool>.Success();
    }
}