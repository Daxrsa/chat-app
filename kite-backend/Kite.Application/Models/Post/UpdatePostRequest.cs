using Kite.Domain.Enums;
using Microsoft.AspNetCore.Http;

namespace Kite.Application.Models.Post;

public class UpdatePostRequest
{
    public string? Title { get; set; }
    public string? Body { get; set; }
    public PostVisibility? Visibility { get; set; }
    public List<string>? Hashtags { get; set; }
    public List<string>? MentionedUsers { get; set; }
    public IFormFileCollection? Files { get; set; } 
}