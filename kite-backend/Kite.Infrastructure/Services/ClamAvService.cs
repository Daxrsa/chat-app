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
        try
        {
            using var memoryStream = new MemoryStream();
            await file.CopyToAsync(memoryStream);
            var fileData = memoryStream.ToArray();

            return await ScanFileAsync(fileData);
        }
        catch (TimeoutException ex)
        {
            return Result<AntivirusScanResult>.Failure(
                new Error("Antivirus.Timeout", $"Antivirus scan timed out {ex.Message}"));
        }
        catch (SocketException ex)
        {
            return Result<AntivirusScanResult>.Failure(
                new Error("Antivirus.ConnectionError", $"Cannot connect to antivirus service {ex.Message}"));
        }
        catch (Exception ex)
        {
            return Result<AntivirusScanResult>.Failure(
                new Error("Antivirus.UnexpectedError", ex.Message));
        }
    }

    private async Task<Result<AntivirusScanResult>> ScanFileAsync(byte[] fileData)
    {
        try
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
                    var virusName = scanResult?.InfectedFiles?.FirstOrDefault()?.VirusName ?? "Unknown virus";
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
        catch (TimeoutException ex)
        {
            return Result<AntivirusScanResult>.Failure(
                new Error("Antivirus.Timeout", $"Antivirus scan timed out {ex.Message}"));
        }
        catch (SocketException ex)
        {
            return Result<AntivirusScanResult>.Failure(
                new Error("Antivirus.ConnectionError", $"Cannot connect to antivirus service {ex.Message}"));
        }
        catch (Exception ex)
        {
            return Result<AntivirusScanResult>.Failure(
                new Error("Antivirus.UnexpectedError", ex.Message));
        }
    }

    private async Task<Result<bool>> IsServiceAvailableAsync()
    {
        try
        {
            await _clamClient.PingAsync();
            return Result<bool>.Success();
        }
        catch (TimeoutException ex)
        {
            return Result<bool>.Failure(
                new Error("Antivirus.Timeout", $"Antivirus service ping timed out {ex.Message}"));
        }
        catch (SocketException ex)
        {
            return Result<bool>.Failure(
                new Error("Antivirus.ConnectionError", $"Cannot connect to antivirus service {ex.Message}"));
        }
        catch (Exception ex)
        {
            return Result<bool>.Failure(
                new Error("Antivirus.PingFailed", $"Antivirus service is not responding {ex.Message}"));
        }
    }
}