namespace BoluBul.Services.Interfaces
{
    public interface ISlugService
    {
        string GenerateSlug(string input);

        Task<string> GenerateUniqueSlugAsync(string input, Func<string, Task<bool>> slugExists);
    }
}
