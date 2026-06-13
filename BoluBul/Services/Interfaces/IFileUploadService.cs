using Microsoft.AspNetCore.Http;

namespace BoluBul.Services.Interfaces
{
    public interface IFileUploadService
    {
        Task<string?> UploadAsync(IFormFile? file, string subFolder, CancellationToken cancellationToken = default);

        Task DeleteAsync(string? fileUrl);
    }
}
