namespace Kite.Application.Models;

public class FileUploadResult
{
    public string Message { get; set; } = string.Empty;
    public string OriginalFileName { get; set; } = string.Empty;
    public string StoredFileName { get; set; } = string.Empty;
    public string FilePath { get; set; } = string.Empty;
    public long FileSize { get; set; }
    public DateTimeOffset UploadedAt { get; set; }
}