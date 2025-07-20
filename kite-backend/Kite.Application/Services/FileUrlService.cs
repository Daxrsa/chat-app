using Kite.Application.Interfaces;
using Microsoft.Extensions.Configuration;

namespace Kite.Application.Services;

public class FileUrlService : IFileUrlService
{
    public string ServeFileUrl(string filePath)
    {
        if (string.IsNullOrEmpty(filePath))
            return string.Empty;

        var uploadsIndex = filePath.IndexOf("uploads", StringComparison.OrdinalIgnoreCase);
        if (uploadsIndex == -1)
            return string.Empty;

        var startIndex = uploadsIndex + "uploads".Length;
        var relativePath = filePath.Substring(startIndex)
            .TrimStart('/', '\\')
            .Replace('\\', '/');

        return $"http://localhost:5019/uploads/{relativePath}";
    }
}