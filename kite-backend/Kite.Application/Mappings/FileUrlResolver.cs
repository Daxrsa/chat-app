using AutoMapper;
using Kite.Application.Interfaces;
using Kite.Application.Models;
using Kite.Domain.Entities;

namespace Kite.Application.Mappings;

public class FileUrlResolver<TDestination>(IFileUrlService fileUrlService)
    : IValueResolver<ApplicationFile, TDestination, string>
{
    public string Resolve(ApplicationFile source, TDestination destination, string destMember,
        ResolutionContext context)
    {
        return fileUrlService.ServeFileUrl(source.FilePath);
    }
}