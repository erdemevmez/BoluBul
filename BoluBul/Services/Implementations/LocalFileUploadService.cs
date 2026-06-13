using BoluBul.Services.Interfaces;

namespace BoluBul.Services.Implementations
{
    public class LocalFileUploadService : IFileUploadService
    {
        private const long MaxFileSizeInBytes = 3 * 1024 * 1024;

        private static readonly HashSet<string> AllowedExtensions = new(StringComparer.OrdinalIgnoreCase)
        {
            ".jpg",
            ".jpeg",
            ".png",
            ".webp"
        };

        private readonly IWebHostEnvironment _environment;

        public LocalFileUploadService(IWebHostEnvironment environment)
        {
            _environment = environment;
        }

        public async Task<string?> UploadAsync(IFormFile? file, string subFolder, CancellationToken cancellationToken = default)
        {
            if (file is null)
            {
                return null;
            }

            if (file.Length == 0)
            {
                throw new InvalidOperationException("Yüklenen dosya boş olamaz.");
            }

            if (file.Length > MaxFileSizeInBytes)
            {
                throw new InvalidOperationException("Dosya boyutu en fazla 3 MB olabilir.");
            }

            var extension = Path.GetExtension(file.FileName);

            if (!AllowedExtensions.Contains(extension))
            {
                throw new InvalidOperationException("Sadece .jpg, .jpeg, .png ve .webp dosyaları yüklenebilir.");
            }

            var safeSubFolder = SanitizeSubFolder(subFolder);
            var uploadsRoot = GetUploadsRoot();
            var targetDirectory = Path.Combine(uploadsRoot, safeSubFolder);

            Directory.CreateDirectory(targetDirectory);

            var safeFileName = $"{Guid.NewGuid():N}{extension.ToLowerInvariant()}";
            var filePath = Path.Combine(targetDirectory, safeFileName);

            await using var stream = new FileStream(filePath, FileMode.CreateNew);
            await file.CopyToAsync(stream, cancellationToken);

            return $"/uploads/{safeSubFolder.Replace(Path.DirectorySeparatorChar, '/')}/{safeFileName}";
        }

        public Task DeleteAsync(string? fileUrl)
        {
            if (string.IsNullOrWhiteSpace(fileUrl))
            {
                return Task.CompletedTask;
            }

            var relativePath = fileUrl.TrimStart('/').Replace('/', Path.DirectorySeparatorChar);

            if (!relativePath.StartsWith("uploads" + Path.DirectorySeparatorChar, StringComparison.OrdinalIgnoreCase))
            {
                return Task.CompletedTask;
            }

            var uploadsRoot = GetUploadsRoot();
            var fullPath = Path.GetFullPath(Path.Combine(GetWebRootPath(), relativePath));

            if (!fullPath.StartsWith(uploadsRoot, StringComparison.OrdinalIgnoreCase))
            {
                return Task.CompletedTask;
            }

            if (File.Exists(fullPath))
            {
                File.Delete(fullPath);
            }

            return Task.CompletedTask;
        }

        private string GetUploadsRoot()
        {
            var uploadsRoot = Path.GetFullPath(Path.Combine(GetWebRootPath(), "uploads"));
            Directory.CreateDirectory(uploadsRoot);

            return uploadsRoot;
        }

        private string GetWebRootPath()
        {
            return _environment.WebRootPath ?? Path.Combine(_environment.ContentRootPath, "wwwroot");
        }

        private static string SanitizeSubFolder(string subFolder)
        {
            var segments = subFolder
                .Split(new[] { '/', '\\' }, StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
                .Select(segment => string.Concat(segment.ToLowerInvariant().Where(ch => char.IsLetterOrDigit(ch) || ch == '-')))
                .Where(segment => !string.IsNullOrWhiteSpace(segment))
                .ToArray();

            return segments.Length == 0
                ? "businesses"
                : Path.Combine(segments);
        }
    }
}
