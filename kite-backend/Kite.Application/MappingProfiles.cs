using AutoMapper;
using Kite.Application.Models;
using Kite.Domain.Entities;

namespace Kite.Application;

public class MappingProfiles : Profile
{
    public MappingProfiles()
    {
        CreateMap<RegisterModel, ApplicationUser>().ReverseMap();
        CreateMap<LoginModel, ApplicationUser>().ReverseMap();
    }
}