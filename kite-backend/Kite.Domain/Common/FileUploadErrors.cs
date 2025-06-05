namespace Kite.Domain.Common;

public static class FileUploadErrors
{
    public static Error NoFile =>
        new Error("FileUpload.NoFile", "No file uploaded.");

    public static Error EmptyFile =>
        new Error("FileUpload.EmptyFile", "The uploaded file is empty.");

    public static Error SizeExceededWithLimit(string maxSizeInBytes) =>
        new Error("FileUpload.SizeExceeded", $"File size exceeds the maximum allowed limit of {maxSizeInBytes}");

    public static Error InvalidExtension =>
        new Error("FileUpload.InvalidExtension", "File extension is not permitted.");

    public static Error InvalidExtensionWithAllowed(string[] allowedExtensions) =>
        new Error("FileUpload.InvalidExtension", $"File extension is not permitted. Allowed extensions: {string.Join(", ", allowedExtensions)}.");

    public static Error VirusScanFailed =>
        new Error("FileUpload.VirusScan", "File failed virus scan and cannot be uploaded.");

    public static Error DirectoryNotFound =>
        new Error("FileUpload.DirectoryNotFound", "Upload directory could not be created or accessed.");

    public static Error AccessDenied =>
        new Error("FileUpload.AccessDenied", "Access denied to upload directory.");

    public static Error IOError(string message) =>
        new Error("FileUpload.IOError", $"File operation failed: {message}");

    public static Error UnknownError(string message) =>
        new Error("FileUpload.UnknownError", $"An unexpected error occurred: {message}");

    public static Error InvalidFileName =>
        new Error("FileUpload.InvalidFileName", "The file name contains invalid characters.");

    public static Error FileAlreadyExists =>
        new Error("FileUpload.FileAlreadyExists", "A file with the same name already exists.");

    public static Error FileAlreadyExistsWithName(string fileName) =>
        new Error("FileUpload.FileAlreadyExists", $"A file with the name '{fileName}' already exists.");

    public static Error DiskSpaceInsufficient =>
        new Error("FileUpload.DiskSpaceInsufficient", "Insufficient disk space to save the file.");

    public static Error PermissionSetFailed =>
        new Error("FileUpload.PermissionSetFailed", "Failed to set security permissions on the uploaded file.");

    public static Error UnsupportedContentType =>
        new Error("FileUpload.UnsupportedContentType", "The file content type is not supported.");

    public static Error UnsupportedContentTypeWithType(string contentType) =>
        new Error("FileUpload.UnsupportedContentType", $"The file content type '{contentType}' is not supported.");

    public static Error CorruptedFile =>
        new Error("FileUpload.CorruptedFile", "The uploaded file appears to be corrupted.");

    public static Error TooManyFiles =>
        new Error("FileUpload.TooManyFiles", "Too many files uploaded at once.");

    public static Error TooManyFilesWithLimit(int maxFiles) =>
        new Error("FileUpload.TooManyFiles", $"Too many files uploaded. Maximum allowed: {maxFiles}.");

    public static Error QuotaExceeded =>
        new Error("FileUpload.QuotaExceeded", "User upload quota has been exceeded.");

    public static Error QuotaExceededWithLimit(long quotaInBytes) =>
        new Error("FileUpload.QuotaExceeded", $"User upload quota of {quotaInBytes / (1024 * 1024)} MB has been exceeded.");

    public static Error ProcessingFailed =>
        new Error("FileUpload.ProcessingFailed", "File processing failed after upload.");

    public static Error ValidationFailed =>
        new Error("FileUpload.ValidationFailed", "File validation failed.");

    public static Error ValidationFailedWithReason(string reason) =>
        new Error("FileUpload.ValidationFailed", $"File validation failed: {reason}");

    public static Error CreationFailed =>
        new Error("FileUpload.CreationFailed", "File creation failed. No changes were made to the storage.");

    public static Error RetrievalError =>
        new Error("FileUpload.RetrievalError", "An error occurred while retrieving the uploaded file information.");

    public static Error DeletionFailed =>
        new Error("FileUpload.DeletionFailed", "Failed to delete the uploaded file.");

    public static Error AntivirusScanFailed(string description) =>
        new Error("FileUpload.AntivirusScanFailed", $"An unexpected error occurred during antivirus file scan {description}.");
    
    public static Error VirusDetected(string virusName) =>
        new Error("FileUpload.VirusDetected", $"A virus has been detected during file scan: {virusName}.");
    
    public static Error UnexpectedError =>
        new Error("FileUpload.UnexpectedError", "An unexpected error occurred during file upload.");
}