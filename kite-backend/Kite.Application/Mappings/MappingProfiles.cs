using AutoMapper;
using Kite.Application.Models;
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
            .ForMember(dest => dest.AuthorProfilePicture, opt => opt.MapFrom(src => src.User.Files.FirstOrDefault(f => f.Type == FileType.ProfilePicture)))
            .ForMember(dest => dest.TimeElapsed, opt => opt.MapFrom(src => Helpers.GetTimeElapsedString(src.CreatedAt)))
            .ForMember(dest => dest.MentionedUsers, opt => opt.MapFrom(src => src.MentionedUsers.FirstOrDefault(f => f.FirstName == src.User.FirstName && f.LastName == src.User.LastName)))
            .ReverseMap();

        CreateMap<ApplicationFile, AttachedFileModel>().ReverseMap();

        CreateMap<FileUploadResult, ApplicationFile>()
            .ForMember(dest => dest.Filename, opt => opt.MapFrom(src => src.StoredFileName))
            .ForMember(dest => dest.Extension, opt => opt.MapFrom(src => Path.GetExtension(src.StoredFileName)))
            .ForMember(dest => dest.Size, opt => opt.MapFrom(src => src.FileSize.ToString()))
            .ForMember(dest => dest.FilePath, opt => opt.MapFrom(src => src.FilePath))
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.Type, opt => opt.Ignore())
            .ForMember(dest => dest.UserId, opt => opt.Ignore())
            .ForMember(dest => dest.PostId, opt => opt.Ignore())
            .ForMember(dest => dest.UploadedAt, opt => opt.Ignore())
            .ForMember(dest => dest.User, opt => opt.Ignore())
            .ForMember(dest => dest.Post, opt => opt.Ignore())
            .ReverseMap();
    }
}