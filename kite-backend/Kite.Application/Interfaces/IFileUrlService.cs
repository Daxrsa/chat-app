namespace Kite.Application.Interfaces;

public interface IFileUrlService
{
    string GetFileUrl(string filePath);
    string GetAbsoluteFileUrl(string filePath, string baseUrl);
}