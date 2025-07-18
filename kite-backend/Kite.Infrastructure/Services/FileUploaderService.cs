using System.Text.Encodings.Web;
using AutoMapper;
using Kite.Application.Interfaces;
using Kite.Application.Models;
using Kite.Application.Utilities;
using Kite.Domain.Common;
using Kite.Domain.Entities;
using Kite.Domain.Enums;
using Kite.Domain.Interfaces;
using Microsoft.AspNetCore.Http;

namespace Kite.Infrastructure.Services;

public class FileUploaderService(
    IApplicationFileRepository fileRepository,
    IUnitOfWork unitOfWork,
    IUserAccessor userAccessor,
    IAntivirusService antivirusService,
    IMapper mapper) : IFileUploaderService
{
    private static readonly string[] PermittedExtensions =
        { ".txt", ".pdf", ".jpg", ".jpeg", ".png", ".gif", ".mp3", ".mp4", ".webp" };

    // 15 MB limit
    private const long FileSizeLimit = 15 * 1024 * 1024;

    private static readonly string
        TargetFilePath = Path.Combine("/home/daorsa/Desktop/KiteUploads");

    private static readonly Dictionary<string, string> FileTypeToFolder = new()
    {
        { ".jpg", "images" },
        { ".jpeg", "images" },
        { ".png", "images" },
        { ".gif", "images" },
        { ".webp", "images" },
        { ".bmp", "images" },
        { ".svg", "images" },
        { ".ico", "images" },
        { ".tiff", "images" },
        { ".tif", "images" },
        { ".heic", "images" },
        { ".heif", "images" },
        { ".avif", "images" },
        { ".mp3", "audio" },
        { ".wav", "audio" },
        { ".flac", "audio" },
        { ".aac", "audio" },
        { ".ogg", "audio" },
        { ".wma", "audio" },
        { ".m4a", "audio" },
        { ".mp4", "video" },
        { ".avi", "video" },
        { ".mov", "video" },
        { ".wmv", "video" },
        { ".flv", "video" },
        { ".webm", "video" },
        { ".mkv", "video" },
        { ".m4v", "video" },
        { ".3gp", "video" },
        { ".txt", "documents" },
        { ".pdf", "documents" },
        { ".doc", "documents" },
        { ".docx", "documents" },
        { ".xls", "documents" },
        { ".xlsx", "documents" },
        { ".ppt", "documents" },
        { ".pptx", "documents" },
        { ".rtf", "documents" },
        { ".odt", "documents" },
        { ".ods", "documents" },
        { ".odp", "documents" },
        { ".zip", "archives" },
        { ".rar", "archives" },
        { ".7z", "archives" },
        { ".tar", "archives" },
        { ".gz", "archives" },
        { ".bz2", "archives" },
        { ".json", "code" },
        { ".xml", "code" },
        { ".html", "code" },
        { ".css", "code" },
        { ".js", "code" },
        { ".ts", "code" },
        { ".cs", "code" },
        { ".java", "code" },
        { ".py", "code" },
        { ".cpp", "code" },
        { ".c", "code" },
        { ".php", "code" },
        { ".sql", "code" }
    };

    public async Task<Result<FileUploadResult>> UploadFileAsync(IFormFile file, FileType type,
        CancellationToken cancellationToken)
    {
        var currentUserId = userAccessor.GetCurrentUserId();

        if (string.IsNullOrEmpty(currentUserId))
        {
            return Result<FileUploadResult>.Failure(
                new Error("Auth.UserNotFound", "Current user ID could not be determined."));
        }

        if (file == null || file.Length == 0)
            return Result<FileUploadResult>.Failure(FileUploadErrors.NoFile);

        if (file.Length > FileSizeLimit)
            return Result<FileUploadResult>.Failure(
                FileUploadErrors.SizeExceededWithLimit(Helpers.FormatFileSize(FileSizeLimit)));

        var avScan = await antivirusService.ScanFileAsync(file);

        if (!avScan.IsSuccess)
        {
            return Result<FileUploadResult>.Failure(avScan.Errors.First().Description);
        }

        if (!avScan.Value.IsClean)
        {
            return Result<FileUploadResult>.Failure(
                FileUploadErrors.VirusDetected(avScan.Value.VirusName));
        }

        var ext = Path.GetExtension(file.FileName).ToLowerInvariant();
        if (string.IsNullOrEmpty(ext) || !PermittedExtensions.Contains(ext))
            return Result<FileUploadResult>.Failure(
                FileUploadErrors.InvalidExtensionWithAllowed(PermittedExtensions));

        var subfolder = FileTypeToFolder.GetValueOrDefault(ext, "others");
        var targetDirectory = Path.Combine(TargetFilePath, subfolder);

        Directory.CreateDirectory(targetDirectory);

        var safeFileName = $"{Guid.NewGuid()}{ext}";
        var finalPath = Path.Combine(targetDirectory, safeFileName);

        await using (var stream = new FileStream(finalPath, FileMode.Create))
        {
            await file.CopyToAsync(stream, cancellationToken);
        }

        // await RemoveExecutePermissionsAsync(finalPath);

        var encodedOriginalName = HtmlEncoder.Default.Encode(Path.GetFileName(file.FileName));
        var fileSize = Helpers.FormatFileSize(file.Length);
        var result = new FileUploadResult
        {
            Message = "File uploaded successfully.",
            OriginalFileName = encodedOriginalName,
            StoredFileName = safeFileName,
            FilePath = finalPath,
            FileSize = fileSize,
            UploadedAt = DateTime.UtcNow,
            UploadedBy = currentUserId,
            Type = type
        };

        var fileInDb = new ApplicationFile
        {
            Filename = safeFileName,
            Extension = ext,
            Size = fileSize,
            UserId = currentUserId,
            FilePath = finalPath,
            UploadedAt = DateTimeOffset.UtcNow,
            Type = type
        };

        await fileRepository.InsertAsync(fileInDb, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);
        return Result<FileUploadResult>.Success(result);
    }

    public async Task<Result<BatchUploadResult>> UploadFilesAsync(
        IFormFileCollection files,
        FileType type,
        CancellationToken cancellationToken)
    {
        var currentUserId = userAccessor.GetCurrentUserId();

        if (string.IsNullOrEmpty(currentUserId))
        {
            return Result<BatchUploadResult>.Failure(
                new Error("Auth.UserNotFound", "Current user ID could not be determined."));
        }

        var uploadStartTime = DateTime.UtcNow;

        if (files == null || !files.Any())
            return Result<BatchUploadResult>.Failure(FileUploadErrors.NoFile);

        var successfulUploads = new List<FileUploadResult>();
        var failedUploads = new List<FailedUpload>();

        var fileIndex = 0;
        foreach (var file in files)
        {
            fileIndex++;

            try
            {
                var uploadResult = await UploadFileAsync(file, type, cancellationToken);

                if (uploadResult.IsSuccess)
                {
                    successfulUploads.Add(uploadResult.Value);
                }
                else
                {
                    var failedUpload = new FailedUpload
                    {
                        FileName = file.FileName ?? "unknown",
                        Reason = uploadResult.Errors.FirstOrDefault()?.Description ??
                                 "Unknown error"
                    };
                    failedUploads.Add(failedUpload);
                }
            }
            catch (Exception ex)
            {
                var failedUpload = new FailedUpload
                {
                    FileName = file.FileName ?? "unknown",
                    Reason = $"Unexpected error during upload - {ex.Message}"
                };
                failedUploads.Add(failedUpload);
            }
        }

        var uploadEndTime = DateTime.UtcNow;
        var totalDuration = (uploadEndTime - uploadStartTime).TotalMilliseconds;

        var batchResult = new BatchUploadResult
        {
            TotalFiles = files.Count,
            SuccessfulUploads = successfulUploads,
            FailedUploads = failedUploads,
            SuccessCount = successfulUploads.Count,
            FailureCount = failedUploads.Count,
            IsPartialSuccess = failedUploads.Count > 0 && successfulUploads.Count > 0,
            UploadedAt = uploadStartTime,
            CompletedAt = uploadEndTime,
            UploadDurationMs = totalDuration,
            UploadedBy = currentUserId
        };

        return Result<BatchUploadResult>.Success(batchResult);
    }

    public async Task<Result<FileDeleteResult>> DeleteFileAsync(Guid fileId,
        CancellationToken cancellationToken)
    {
        var currentUserId = userAccessor.GetCurrentUserId();

        if (string.IsNullOrEmpty(currentUserId))
        {
            return Result<FileDeleteResult>.Failure(
                new Error("Auth.UserNotFound", "Current user ID could not be determined."));
        }

        var fileEntity = await fileRepository.GetByIdAsync(fileId, cancellationToken);
        if (fileEntity == null)
        {
            return Result<FileDeleteResult>.Failure(
                new Error("File.NotFound", "File not found."));
        }

        if (fileEntity.UserId != currentUserId)
        {
            return Result<FileDeleteResult>.Failure(
                new Error("File.Unauthorized", "You are not authorized to delete this file."));
        }

        var deleteResult = new FileDeleteResult
        {
            FileName = fileEntity.Filename,
            DeletedBy = currentUserId,
            DeletedAt = DateTime.UtcNow,
            PhysicalFileDeleted = false,
            DatabaseRecordDeleted = false
        };

        try
        {
            var subfolder = FileTypeToFolder.GetValueOrDefault(fileEntity.Extension, "others");
            var filePath = Path.Combine(TargetFilePath, subfolder, fileEntity.Filename);

            if (File.Exists(filePath))
            {
                File.Delete(filePath);
                deleteResult.PhysicalFileDeleted = true;
            }
            else
            {
                deleteResult.PhysicalFileDeleted = false;
            }
        }
        catch
        {
            deleteResult.PhysicalFileDeleted = false;
        }

        try
        {
            await fileRepository.DeleteAsync(fileEntity, cancellationToken);
            await unitOfWork.SaveChangesAsync(cancellationToken);
            deleteResult.DatabaseRecordDeleted = true;
        }
        catch (Exception ex)
        {
            return Result<FileDeleteResult>.Failure(
                new Error("File.DatabaseError",
                    $"Failed to delete file record from database: {ex.Message}"));
        }

        if (deleteResult.PhysicalFileDeleted && deleteResult.DatabaseRecordDeleted)
        {
            deleteResult.Message = "File deleted successfully.";
        }
        else if (deleteResult.DatabaseRecordDeleted)
        {
            deleteResult.Message =
                "File record deleted from database. Physical file was not found.";
        }
        else
        {
            deleteResult.Message = "Database record deleted but physical file deletion failed.";
        }

        return Result<FileDeleteResult>.Success(deleteResult);
    }

    public async Task<Result<AttachedFileModel>> ServeFileAsync(Guid fileId, CancellationToken cancellationToken = default)
    {
        var file = await fileRepository.GetByIdAsync(fileId, cancellationToken);
        if (file == null)
        {
            return Result<AttachedFileModel>.Failure(
                new Error("File.NotFound", "File not found."));
        }
    
        var fileModel = mapper.Map<AttachedFileModel>(file);
    
        return Result<AttachedFileModel>.Success(fileModel);
    }

    public async Task<Result<IEnumerable<AttachedFileModel>>> GetAllFilesAsync(CancellationToken cancellationToken = default)
    {
        var files = await fileRepository.GetAllAsync(cancellationToken);
        if (files == null)
        {
            return Result<IEnumerable<AttachedFileModel>>.Failure(
                new Error("Files.NotFound", "No files found"));
        }
        var filesModel = mapper.Map<IEnumerable<AttachedFileModel>>(files);
        
        return Result<IEnumerable<AttachedFileModel>>.Success(filesModel);
    }
}

// public async Task RemoveExecutePermissionsAsync(string filePath)
// {
//     try
//     {
//         // Remove execute permissions on Linux (chmod 644)
//         var process = new Process
//         {
//             StartInfo = new ProcessStartInfo
//             {
//                 FileName = "chmod",
//                 Arguments = $"644 \"{filePath}\"",
//                 UseShellExecute = false,
//                 CreateNoWindow = true,
//                 RedirectStandardOutput = true,
//                 RedirectStandardError = true
//             }
//         };
//
//         process.Start();
//         await process.WaitForExitAsync();
//         
//         if (process.ExitCode != 0)
//         {
//             var error = await process.StandardError.ReadToEndAsync();
//             logger.LogWarning($"Failed to remove execute permissions from {filePath}: {error}");
//         }
//     }
//     catch (Exception ex)
//     {
//         logger.LogWarning($"Exception while removing execute permissions from {filePath}: {ex.Message}");
//     }
// }