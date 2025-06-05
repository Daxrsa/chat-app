namespace Kite.Domain.Common;

public class FileUploadError
{
    public string FileName { get; set; } = string.Empty;
    public string ErrorCode { get; set; } = string.Empty;
    public string ErrorMessage { get; set; } = string.Empty;
    public List<Error> Errors { get; set; } = new();
}