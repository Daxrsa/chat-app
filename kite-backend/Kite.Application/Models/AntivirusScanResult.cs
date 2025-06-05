namespace Kite.Application.Models;

public class AntivirusScanResult
{
    public bool IsClean { get; set; }
    public bool IsError { get; set; }
    public string? VirusName { get; set; }
    public string? ErrorMessage { get; set; }
}