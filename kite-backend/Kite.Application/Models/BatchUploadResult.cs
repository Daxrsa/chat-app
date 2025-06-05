using Kite.Domain.Common;

namespace Kite.Application.Models;

public class BatchUploadResult
{
    public int TotalFiles { get; set; }
    public List<FileUploadResult> SuccessfulUploads { get; set; } = new();
    public List<FailedUpload> FailedUploads { get; set; } = new();
    public int SuccessCount { get; set; }
    public int FailureCount { get; set; }
    public bool IsPartialSuccess { get; set; }
    public bool IsCompleteSuccess => FailureCount == 0;
    public DateTime UploadedAt { get; set; }
    public DateTime CompletedAt { get; set; }
    public double UploadDurationMs { get; set; }
    public List<FileUploadError> FailedFiles { get; set; } = new();
    public string UploadedBy { get; set; } = string.Empty;
    public string Summary => $"Uploaded {SuccessCount}/{TotalFiles} files successfully";
    public string DetailedSummary => IsCompleteSuccess 
        ? $"All {TotalFiles} files uploaded successfully" 
        : $"Batch upload: {SuccessCount} successful, {FailureCount} failed";
}