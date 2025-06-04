// using System.Text.Encodings.Web;
// using Microsoft.AspNetCore.Mvc;
//
// namespace Kite.Web.Controllers;
//
// public class FileController : BaseApiController
// {
//     private static readonly string[] _permittedExtensions = { ".txt", ".pdf", ".jpg", ".jpeg", ".png", ".gif" };
//     // 10 MB limit
//     private const long _fileSizeLimit = 10 * 1024 * 1024;
//     private static readonly string _targetFilePath = Path.Combine("/home/daorsa/Desktop/KiteUploads");
//
//     [HttpPost("upload")]
//     [RequestSizeLimit(_fileSizeLimit)]
//     public async Task<IActionResult> UploadFile(IFormFile file, CancellationToken cancellationToken)
//     {
//         if (file == null || file.Length == 0)
//             return BadRequest("No file uploaded.");
//         
//         if (file.Length > _fileSizeLimit)
//             return BadRequest("File size exceeds limit.");
//         
//         var ext = Path.GetExtension(file.FileName).ToLowerInvariant();
//         if (string.IsNullOrEmpty(ext) || !_permittedExtensions.Contains(ext))
//             return BadRequest("File extension not permitted.");
//
//         // Virus scan placeholder (implement with your AV solution)
//         // var isClean = await MyAntivirusScanner.IsCleanAsync(file.OpenReadStream());
//         // if (!isClean) return BadRequest("File failed virus scan.");
//
//         // Ensure upload directory exists and has no execute permissions (set via OS)
//         Directory.CreateDirectory(_targetFilePath);
//
//         // Generate a safe file name (GUID + extension)
//         var safeFileName = $"{Guid.NewGuid()}{ext}";
//         var finalPath = Path.Combine(_targetFilePath, safeFileName);
//
//         // Optionally check for duplicate names, but GUIDs are unique
//
//         // Save file
//         using (var stream = new FileStream(finalPath, FileMode.Create))
//         {
//             await file.CopyToAsync(stream, cancellationToken);
//         }
//
//         // Log/display original file name safely if needed
//         var encodedOriginalName = HtmlEncoder.Default.Encode(Path.GetFileName(file.FileName));
//
//         return Ok(new
//         {
//             Message = "File uploaded successfully.",
//             OriginalFileName = encodedOriginalName,
//             StoredFileName = safeFileName
//         });
//     }
// }