namespace Kite.Application.Models;

public class FailedUpload
{
    public string FileName { get; set; } = string.Empty;
    public string Reason { get; set; } = string.Empty;
}