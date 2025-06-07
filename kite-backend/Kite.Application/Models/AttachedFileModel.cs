using Kite.Application.Utilities;

namespace Kite.Application.Models;

public class AttachedFileModel
{
    public Guid Id { get; set; }
    public string FileName { get; set; } = string.Empty;
    public string Url { get; set; } = string.Empty;
    public string FileType { get; set; } = string.Empty;
    public long FileSizeInBytes { get; set; }
    public DateTimeOffset UploadedAt { get; set; }
    public string FileSizeFormatted => Helpers.FormatFileSize(FileSizeInBytes);
    private string ContentType { get; set; } = string.Empty;
    public bool IsImage => ContentType.StartsWith("image/");
    public bool IsVideo => ContentType.StartsWith("video/");
    public bool IsAudio => ContentType.StartsWith("audio/");

    public bool IsDocument => ContentType.StartsWith("application/") ||
                              ContentType.StartsWith("text/") ||
                              ContentType.Contains("document") ||
                              ContentType.Contains("spreadsheet") ||
                              ContentType.Contains("presentation");

    public bool IsArchive => ContentType.StartsWith("application/zip") ||
                             ContentType.StartsWith("application/x-rar") ||
                             ContentType.StartsWith("application/x-7z") ||
                             ContentType.StartsWith("application/x-tar");

    public bool IsCode => ContentType.StartsWith("text/") ||
                          ContentType.Contains("javascript") ||
                          ContentType.Contains("json") ||
                          ContentType.Contains("xml");
}