namespace Kite.Application.Utilities;

public static class Helpers
{
    public static string GetTimeElapsedString(DateTimeOffset requestTime)
    {
        var currentTime = DateTimeOffset.UtcNow;
        var timeSpan = currentTime - requestTime;

        return timeSpan switch
        {
            var ts when ts.TotalDays > 365 =>
                (int)(ts.TotalDays / 365) == 1
                    ? "1 year ago"
                    : $"{(int)(ts.TotalDays / 365)} years ago",

            var ts when ts.TotalDays > 30 =>
                (int)(ts.TotalDays / 30) == 1
                    ? "1 month ago"
                    : $"{(int)(ts.TotalDays / 30)} months ago",

            var ts when ts.TotalDays > 7 =>
                (int)(ts.TotalDays / 7) == 1
                    ? "1 week ago"
                    : $"{(int)(ts.TotalDays / 7)} weeks ago",

            var ts when ts.TotalDays >= 1 =>
                (int)ts.TotalDays == 1 ? "yesterday" : $"{(int)ts.TotalDays} days ago",

            var ts when ts.TotalHours >= 1 =>
                (int)ts.TotalHours == 1 ? "an hour ago" : $"{(int)ts.TotalHours} hours ago",

            var ts when ts.TotalMinutes >= 1 =>
                (int)ts.TotalMinutes == 1 ? "a minute ago" : $"{(int)ts.TotalMinutes} minutes ago",

            var ts when ts.TotalSeconds >= 30 => "less than a minute ago",

            _ => "just now"
        };
    }

    public static string FormatFileSize(long sizeInBytes)
    {
        if (sizeInBytes == 0)
            return "0 B";

        string[] units = { "B", "KB", "MB", "GB", "TB", "PB" };
        double size = sizeInBytes;
        int unitIndex = 0;

        while (size >= 1024 && unitIndex < units.Length - 1)
        {
            size /= 1024;
            unitIndex++;
        }

        return unitIndex == 0
            ? $"{size:F0} {units[unitIndex]}"
            : $"{size:F2} {units[unitIndex]}";
    }

    public static string? GetContentType(string extension)
    {
        return extension.ToLowerInvariant() switch
        {
            // Images
            ".jpg" or ".jpeg" => "image/jpeg",
            ".png" => "image/png",
            ".gif" => "image/gif",
            ".webp" => "image/webp",
            ".bmp" => "image/bmp",
            ".svg" => "image/svg+xml",
            ".ico" => "image/x-icon",
            ".tiff" or ".tif" => "image/tiff",
            ".heic" => "image/heic",
            ".heif" => "image/heif",
            ".avif" => "image/avif",

            // Audio
            ".mp3" => "audio/mpeg",
            ".wav" => "audio/wav",
            ".flac" => "audio/flac",
            ".aac" => "audio/aac",
            ".ogg" => "audio/ogg",
            ".wma" => "audio/x-ms-wma",
            ".m4a" => "audio/mp4",

            // Video
            ".mp4" => "video/mp4",
            ".avi" => "video/x-msvideo",
            ".mov" => "video/quicktime",
            ".wmv" => "video/x-ms-wmv",
            ".flv" => "video/x-flv",
            ".webm" => "video/webm",
            ".mkv" => "video/x-matroska",
            ".m4v" => "video/x-m4v",
            ".3gp" => "video/3gpp",

            // Documents
            ".txt" => "text/plain",
            ".pdf" => "application/pdf",
            ".doc" => "application/msword",
            ".docx" => "application/vnd.openxmlformats-officedocument.wordprocessingml.document",
            ".xls" => "application/vnd.ms-excel",
            ".xlsx" => "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
            ".ppt" => "application/vnd.ms-powerpoint",
            ".pptx" => "application/vnd.openxmlformats-officedocument.presentationml.presentation",
            ".rtf" => "application/rtf",
            ".odt" => "application/vnd.oasis.opendocument.text",
            ".ods" => "application/vnd.oasis.opendocument.spreadsheet",
            ".odp" => "application/vnd.oasis.opendocument.presentation",

            // Archives
            ".zip" => "application/zip",
            ".rar" => "application/vnd.rar",
            ".7z" => "application/x-7z-compressed",
            ".tar" => "application/x-tar",
            ".gz" => "application/gzip",
            ".bz2" => "application/x-bzip2",

            // Code
            ".json" => "application/json",
            ".xml" => "application/xml",
            ".html" => "text/html",
            ".css" => "text/css",
            ".js" => "application/javascript",
            ".ts" => "application/typescript",
            ".cs" => "text/plain",
            ".java" => "text/plain",
            ".py" => "text/plain",
            ".cpp" => "text/plain",
            ".c" => "text/plain",
            ".php" => "text/plain",
            ".sql" => "text/plain",

            _ => "application/octet-stream"
        };
    }
}