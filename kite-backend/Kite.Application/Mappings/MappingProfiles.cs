using AutoMapper;
using Kite.Application.Models;
using Kite.Application.Models.Post;
using Kite.Application.Utilities;
using Kite.Domain.Entities;
using Kite.Domain.Enums;

namespace Kite.Application.Mappings;

public class MappingProfiles : Profile
{
    public MappingProfiles()
    {
        CreateMap<Post, PostModel>()
            .ForMember(dest => dest.AuthorFirstName, opt => opt.MapFrom(src => src.User.FirstName))
            .ForMember(dest => dest.AuthorLastName, opt => opt.MapFrom(src => src.User.LastName))
            .ForMember(dest => dest.AuthorUserName, opt => opt.MapFrom(src => src.User.UserName))
            .ForMember(dest => dest.AuthorProfilePicture,
                opt => opt.MapFrom(src =>
                    src.User.Files.FirstOrDefault(f => f.Type == FileType.ProfilePicture)))
            .ForMember(dest => dest.TimeElapsed,
                opt => opt.MapFrom(src => Helpers.GetTimeElapsedString(src.CreatedAt)))
            .ForMember(dest => dest.MentionedUsers,
                opt => opt.MapFrom(src => src.MentionedUsers.FirstOrDefault(f =>
                    f.FirstName == src.User.FirstName && f.LastName == src.User.LastName)))
            .ReverseMap();

        CreateMap<ApplicationFile, AttachedFileModel>()
            .ForMember(dest => dest.FilePath, opt => opt.MapFrom<FileUrlResolver<AttachedFileModel>>())
            .ReverseMap();

        CreateMap<ApplicationFile, UserModel>() //TODO not working as expected
            .ForMember(dest => dest.ProfilePicture,
                opt => opt.MapFrom<FileUrlResolver<UserModel>>());

        CreateMap<Post, CreatePostRequest>()
            .ForMember(dest => dest.MentionedUsers, opt => opt.MapFrom(src =>
                src.MentionedUsers.Select(u => new UserModel()
                {
                    Id = u.Id,
                    FirstName = u.FirstName,
                    LastName = u.LastName
                })))
            .ReverseMap();
    }
}