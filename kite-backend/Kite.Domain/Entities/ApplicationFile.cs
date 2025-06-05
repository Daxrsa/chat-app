namespace Kite.Domain.Entities;

public class ApplicationFile
{
    public Guid Id { get; set; }
    public string Filename { get; set; }
    public string Extension { get; set; }
    public long Size { get; set; }
    public string UserId { get; set; }
    public ApplicationUser User { get; set; }
    public DateTimeOffset UploadedAt { get; set; }
}