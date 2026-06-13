using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;
using BoluBul.Services.Interfaces;

namespace BoluBul.Services.Implementations
{
    public class SlugService : ISlugService
    {
        public string GenerateSlug(string input)
        {
            if (string.IsNullOrWhiteSpace(input))
            {
                return "n-a";
            }

            var transliterated = TransliterateTurkishCharacters(input.Trim());
            var normalized = transliterated.Normalize(NormalizationForm.FormD);
            var builder = new StringBuilder(normalized.Length);

            foreach (var character in normalized)
            {
                var category = CharUnicodeInfo.GetUnicodeCategory(character);

                if (category != UnicodeCategory.NonSpacingMark)
                {
                    builder.Append(character);
                }
            }

            var slug = builder.ToString().Normalize(NormalizationForm.FormC).ToLowerInvariant();
            slug = Regex.Replace(slug, @"[^a-z0-9\s-]", " ");
            slug = Regex.Replace(slug, @"[\s-]+", "-").Trim('-');

            return string.IsNullOrWhiteSpace(slug) ? "n-a" : slug;
        }

        public async Task<string> GenerateUniqueSlugAsync(string input, Func<string, Task<bool>> slugExists)
        {
            var baseSlug = GenerateSlug(input);
            var slug = baseSlug;
            var counter = 2;

            while (await slugExists(slug))
            {
                slug = $"{baseSlug}-{counter}";
                counter++;
            }

            return slug;
        }

        private static string TransliterateTurkishCharacters(string value)
        {
            var builder = new StringBuilder(value.Length);

            foreach (var character in value)
            {
                builder.Append(character switch
                {
                    'ç' or 'Ç' => 'c',
                    'ğ' or 'Ğ' => 'g',
                    'ı' or 'I' or 'İ' => 'i',
                    'ö' or 'Ö' => 'o',
                    'ş' or 'Ş' => 's',
                    'ü' or 'Ü' => 'u',
                    _ => character
                });
            }

            return builder.ToString();
        }
    }
}
