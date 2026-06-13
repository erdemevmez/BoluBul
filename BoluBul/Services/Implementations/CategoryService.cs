using BoluBul.Data;
using BoluBul.Models;
using BoluBul.Repositories.Interfaces;
using BoluBul.Services.Interfaces;
using BoluBul.ViewModels;
using BoluBul.ViewModels.Admin;
using Microsoft.EntityFrameworkCore;

namespace BoluBul.Services.Implementations
{
    public class CategoryService : ICategoryService
    {
        private readonly ApplicationDbContext _context;
        private readonly ICategoryRepository _categoryRepository;
        private readonly ISlugService _slugService;

        public CategoryService(
            ApplicationDbContext context,
            ICategoryRepository categoryRepository,
            ISlugService slugService)
        {
            _context = context;
            _categoryRepository = categoryRepository;
            _slugService = slugService;
        }

        public Task<IReadOnlyList<CategoryCardViewModel>> GetActiveCategoriesAsync()
        {
            return GetCategoryWithBusinessCountAsync();
        }

        public async Task<IReadOnlyList<CategoryCardViewModel>> GetCategoryWithBusinessCountAsync()
        {
            return await _categoryRepository.QueryActive()
                .Select(c => new CategoryCardViewModel
                {
                    Id = c.Id,
                    Name = c.Name,
                    Slug = c.Slug,
                    Icon = c.Icon,
                    Description = GetCategoryDescription(c.Name),
                    BusinessCount = c.Businesses.Count(b => b.IsApproved && b.IsActive)
                })
                .ToListAsync();
        }

        public async Task<CategoryListViewModel> GetCategoryListAsync()
        {
            return new CategoryListViewModel
            {
                Categories = await GetCategoryWithBusinessCountAsync()
            };
        }

        public async Task<BusinessListViewModel?> GetCategoryDetailAsync(string slug, string? sort = null)
        {
            var category = await _context.Categories
                .AsNoTracking()
                .FirstOrDefaultAsync(c => c.Slug == slug && c.IsActive);

            if (category is null)
            {
                return null;
            }

            var query = _context.Businesses
                .AsNoTracking()
                .Include(b => b.Category)
                .Include(b => b.District)
                .Include(b => b.Stats)
                .Where(b => b.CategoryId == category.Id && b.IsApproved && b.IsActive);

            query = sort switch
            {
                "rating" => query.OrderByDescending(b => b.AverageRating),
                "popular" => query.OrderByDescending(b => b.Stats != null ? b.Stats.ViewCount : 0),
                _ => query.OrderByDescending(b => b.CreatedAt)
            };

            var businesses = await query
                .Select(b => new BusinessCardViewModel
                {
                    Id = b.Id,
                    Name = b.Name,
                    Slug = b.Slug,
                    CategoryName = b.Category.Name,
                    DistrictName = b.District.Name,
                    ShortDescription = b.ShortDescription,
                    LogoUrl = b.LogoUrl,
                    Phone = b.Phone,
                    WhatsApp = b.WhatsApp,
                    AverageRating = b.AverageRating,
                    ReviewCount = b.ReviewCount,
                    IsFeatured = b.IsFeatured,
                    Address = b.Address
                })
                .ToListAsync();

            return new BusinessListViewModel
            {
                Category = category.Slug,
                Sort = string.IsNullOrWhiteSpace(sort) ? "newest" : sort,
                TotalCount = businesses.Count,
                Businesses = businesses,
                Categories = await GetCategoryWithBusinessCountAsync(),
                Districts = await _context.Districts
                    .AsNoTracking()
                    .OrderBy(d => d.Name)
                    .Select(d => new DistrictFilterViewModel { Id = d.Id, Name = d.Name })
                    .ToListAsync()
            };
        }

        public async Task<IReadOnlyList<AdminCategoryRowViewModel>> GetAdminCategoriesAsync()
        {
            return await _context.Categories
                .AsNoTracking()
                .OrderBy(c => c.DisplayOrder)
                .Select(c => new AdminCategoryRowViewModel
                {
                    Id = c.Id,
                    Name = c.Name,
                    Slug = c.Slug,
                    Icon = c.Icon,
                    DisplayOrder = c.DisplayOrder,
                    IsActive = c.IsActive,
                    BusinessCount = c.Businesses.Count
                })
                .ToListAsync();
        }

        public async Task<int> CreateCategoryAsync(string name, string? icon, int displayOrder)
        {
            var slug = await _slugService.GenerateUniqueSlugAsync(name, s => _context.Categories.AnyAsync(c => c.Slug == s));

            var category = new Category
            {
                Name = name,
                Slug = slug,
                Icon = icon,
                DisplayOrder = displayOrder,
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            };

            await _categoryRepository.AddAsync(category);
            await _categoryRepository.SaveChangesAsync();

            return category.Id;
        }

        public async Task<bool> UpdateCategoryAsync(int id, string name, string? icon, int displayOrder, bool isActive)
        {
            var category = await _context.Categories.FindAsync(id);

            if (category is null)
            {
                return false;
            }

            category.Name = name;
            category.Icon = icon;
            category.DisplayOrder = displayOrder;
            category.IsActive = isActive;
            category.Slug = await _slugService.GenerateUniqueSlugAsync(
                name,
                s => _context.Categories.AnyAsync(c => c.Slug == s && c.Id != id));

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> ToggleActiveAsync(int id)
        {
            var category = await _context.Categories.FindAsync(id);

            if (category is null)
            {
                return false;
            }

            category.IsActive = !category.IsActive;
            await _context.SaveChangesAsync();

            return true;
        }

        private static string GetCategoryDescription(string name)
        {
            return name switch
            {
                "Yiyecek & \u0130\u00e7ecek" => "Restoran, kafe ve lezzet durakları",
                "Otomotiv" => "Servis, bakım ve araç hizmetleri",
                "G\u00fczellik & Bak\u0131m" => "Kuaför, güzellik ve bakım noktaları",
                "Sa\u011fl\u0131k" => "Sağlık ve yaşam hizmetleri",
                "E\u011fitim" => "Kurs, okul ve eğitim kurumları",
                "Giyim & Al\u0131\u015fveri\u015f" => "Mağaza ve alışveriş noktaları",
                "Ev Hizmetleri" => "Ev, bakım ve teknik destek hizmetleri",
                "Konaklama" => "Otel, pansiyon ve konaklama yerleri",
                "Spor & Fitness" => "Spor salonları ve aktif yaşam noktaları",
                "Teknoloji" => "Teknik servis ve teknoloji hizmetleri",
                "Resmi Kurumlar" => "Kamu hizmetleri ve resmi kurumlar",
                "Di\u011fer" => "Diğer yerel işletme ve hizmetler",
                _ => "Bolu’da yerel işletmeleri keşfet"
            };
        }
    }
}
