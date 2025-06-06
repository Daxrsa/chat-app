namespace Kite.Application.Models;

public class FileDeleteResult
{
    public string Message { get; set; } = string.Empty;
    public string FileName { get; set; } = string.Empty;
    public string DeletedBy { get; set; } = string.Empty;
    public DateTimeOffset DeletedAt { get; set; }
    public bool PhysicalFileDeleted { get; set; }
    public bool DatabaseRecordDeleted { get; set; }
}