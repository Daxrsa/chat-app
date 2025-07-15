using Kite.Application.Interfaces;
using Kite.Application.Models;
using Kite.Domain.Common;
using nClam;
using System.Net.Sockets;
using Microsoft.AspNetCore.Http;

namespace Kite.Infrastructure.Services;

public class ClamAvService : IAntivirusService
{
    private readonly ClamClient _clamClient = new("localhost", 3310);

    public async Task<Result<AntivirusScanResult>> ScanFileAsync(IFormFile file)
    {
        using var memoryStream = new MemoryStream();
        await file.CopyToAsync(memoryStream);
        var fileData = memoryStream.ToArray();
        return await ScanFileAsync(fileData);
    }

    private async Task<Result<AntivirusScanResult>> ScanFileAsync(byte[] fileData)
    {
        var pingResult = await IsServiceAvailableAsync();
        if (!pingResult.IsSuccess)
        {
            return Result<AntivirusScanResult>.Failure(
                new Error("Antivirus.ServiceUnavailable", "Antivirus service is not available"));
        }

        var scanResult = await _clamClient.SendAndScanFileAsync(fileData);

        switch (scanResult.Result)
        {
            case ClamScanResults.Clean:
                return Result<AntivirusScanResult>.Success(
                    new AntivirusScanResult { IsClean = true });

            case ClamScanResults.VirusDetected:
                var virusName = scanResult?.InfectedFiles?.FirstOrDefault()?.VirusName ??
                                "Unknown virus";
                return Result<AntivirusScanResult>.Success(
                    new AntivirusScanResult
                    {
                        IsClean = false,
                        VirusName = virusName
                    });

            case ClamScanResults.Error:
                return Result<AntivirusScanResult>.Failure(
                    new Error("Antivirus.ScanError", $"Scan failed: {scanResult.RawResult}"));

            default:
                return Result<AntivirusScanResult>.Failure(
                    new Error("Antivirus.UnknownResult", "Unknown scan result"));
        }
    }

    private async Task<Result<bool>> IsServiceAvailableAsync()
    {
        var ping = await _clamClient.PingAsync();
        if (ping is false)
        {
            return Result<bool>.Failure(
                new Error("Antivirus.ServiceUnavailable", "Antivirus service is not available"));
        }

        return Result<bool>.Success();
    }
}