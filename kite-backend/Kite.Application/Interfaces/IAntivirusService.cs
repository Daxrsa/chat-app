using Kite.Application.Models;
using Kite.Domain.Common;
using Microsoft.AspNetCore.Http;

namespace Kite.Application.Interfaces;

public interface IAntivirusService
{
    Task<Result<AntivirusScanResult>> ScanFileAsync(IFormFile file);
}