using Kite.Application.Interfaces;
using Microsoft.Extensions.Configuration;

namespace Kite.Application.Services;

public class FileUrlService(IConfiguration configuration) : IFileUrlService
{
    private readonly string _uploadPath = configuration["FileStorage:UploadPath"] ?? "KiteUploads";

    public string GetFileUrl(string filePath)
    {
        if (string.IsNullOrEmpty(filePath))
            return string.Empty;

        var relativePath = filePath.Replace(_uploadPath, "").TrimStart('/', '\\');
        return $"/uploads/{relativePath.Replace('\\', '/')}";
    }

    public string GetAbsoluteFileUrl(string filePath, string baseUrl)
    {
        var relativeUrl = GetFileUrl(filePath);
        return string.IsNullOrEmpty(relativeUrl)
            ? string.Empty
            : $"{baseUrl.TrimEnd('/')}{relativeUrl}";
    }
}