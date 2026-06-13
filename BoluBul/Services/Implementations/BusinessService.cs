using BoluBul.Data;
using BoluBul.Models;
using BoluBul.Repositories.Interfaces;
using BoluBul.Services.Interfaces;
using BoluBul.ViewModels;
using BoluBul.ViewModels.Admin;
using BoluBul.ViewModels.Owner;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace BoluBul.Services.Implementations
{
    public class BusinessService : IBusinessService
    {
        private readonly ApplicationDbContext _context;
        private readonly IBusinessRepository _businessRepository;
        private readonly ISlugService _slugService;
        private readonly IFileUploadService _fileUploadService;
        private readonly ICategoryService _categoryService;

        public BusinessService(
            ApplicationDbContext context,
            IBusinessRepository businessRepository,
            ISlugService slugService,
            IFileUploadService fileUploadService,
            ICategoryService categoryService)
        {
            _context = context;
            _businessRepository = businessRepository;
            _slugService = slugService;
            _fileUploadService = fileUploadService;
            _categoryService = categoryService;
        }

        public async Task<IReadOnlyList<BusinessCardViewModel>> GetFeaturedBusinessesAsync(int take = 6)
        {
            return await _businessRepository.QueryPublic()
                .Where(b => b.IsFeatured)
                .OrderByDescending(b => b.AverageRating)
                .ThenBy(b => b.Name)
                .Take(take)
                .Select(b => ToCardProjection(b))
                .ToListAsync();
        }

        public async Task<IReadOnlyList<BusinessCardViewModel>> GetLatestBusinessesAsync(int take = 6)
        {
            return await _businessRepository.QueryPublic()
                .OrderByDescending(b => b.CreatedAt)
                .Take(take)
                .Select(b => ToCardProjection(b))
                .ToListAsync();
        }

        public async Task<BusinessListViewModel> SearchBusinessesAsync(string? search, string? category, int? districtId, string? sort)
        {
            var query = _businessRepository.QueryPublic();

            if (!string.IsNullOrWhiteSpace(search))
            {
                var term = search.Trim();
                query = query.Where(b =>
                    b.Name.Contains(term) ||
                    b.Description.Contains(term) ||
                    b.Category.Name.Contains(term));
            }

            if (!string.IsNullOrWhiteSpace(category))
            {
                query = query.Where(b => b.Category.Slug == category);
            }

            if (districtId.HasValue)
            {
                query = query.Where(b => b.DistrictId == districtId.Value);
            }

            query = sort switch
            {
                "rating" => query.OrderByDescending(b => b.AverageRating).ThenByDescending(b => b.ReviewCount),
                "popular" => query.OrderByDescending(b => b.Stats != null ? b.Stats.ViewCount : 0),
                _ => query.OrderByDescending(b => b.CreatedAt)
            };

            var businesses = await query
                .Select(b => ToCardProjection(b))
                .ToListAsync();

            return new BusinessListViewModel
            {
                Search = search,
                Category = category,
                DistrictId = districtId,
                Sort = string.IsNullOrWhiteSpace(sort) ? "newest" : sort,
                TotalCount = businesses.Count,
                Businesses = businesses,
                Categories = await _categoryService.GetCategoryWithBusinessCountAsync(),
                Districts = await GetDistrictFiltersAsync()
            };
        }

        public async Task<BusinessDetailViewModel?> GetBusinessDetailBySlugAsync(string slug, string? currentUserId = null)
        {
            var business = await _businessRepository.GetPublicBySlugAsync(slug);

            if (business is null)
            {
                return null;
            }

            var isFavorite = !string.IsNullOrWhiteSpace(currentUserId) &&
                await _context.Favorites.AnyAsync(f => f.BusinessId == business.Id && f.UserId == currentUserId);

            return new BusinessDetailViewModel
            {
                Id = business.Id,
                Name = business.Name,
                Slug = business.Slug,
                CategoryName = business.Category.Name,
                CityName = business.City.Name,
                DistrictName = business.District.Name,
                NeighborhoodName = business.Neighborhood?.Name,
                Description = business.Description,
                ShortDescription = business.ShortDescription,
                Phone = business.Phone,
                WhatsApp = business.WhatsApp,
                Email = business.Email,
                WebsiteUrl = business.WebsiteUrl,
                Address = business.Address,
                Latitude = business.Latitude,
                Longitude = business.Longitude,
                LogoUrl = business.LogoUrl,
                CoverImageUrl = business.CoverImageUrl,
                AverageRating = business.AverageRating,
                ReviewCount = business.ReviewCount,
                IsFavorite = isFavorite,
                Images = business.Images
                    .OrderBy(i => i.DisplayOrder)
                    .Select(i => new BusinessImageViewModel
                    {
                        Id = i.Id,
                        ImageUrl = i.ImageUrl,
                        AltText = i.AltText
                    })
                    .ToList(),
                Hours = business.Hours
                    .OrderBy(h => h.DayOfWeek)
                    .Select(h => new BusinessHourViewModel
                    {
                        DayOfWeek = h.DayOfWeek,
                        OpenTime = h.OpenTime,
                        CloseTime = h.CloseTime,
                        IsClosed = h.IsClosed
                    })
                    .ToList(),
                Reviews = business.Reviews
                    .OrderByDescending(r => r.CreatedAt)
                    .Select(r => new ReviewItemViewModel
                    {
                        Id = r.Id,
                        UserDisplayName = r.User.FullName ?? r.User.Email ?? "BoluBul kullanıcısı",
                        Rating = r.Rating,
                        Comment = r.Comment,
                        CreatedAt = r.CreatedAt
                    })
                    .ToList(),
                ReviewForm = new ReviewCreateViewModel
                {
                    BusinessId = business.Id,
                    BusinessSlug = business.Slug,
                    Rating = 5
                }
            };
        }

        public async Task<BusinessCreateViewModel> BuildCreateViewModelAsync(BusinessCreateViewModel? model = null)
        {
            model ??= new BusinessCreateViewModel();
            model.Categories = await GetCategorySelectListAsync(model.CategoryId);
            model.Districts = await GetDistrictSelectListAsync(model.DistrictId);
            model.Neighborhoods = await GetNeighborhoodSelectListAsync(model.NeighborhoodId);

            return model;
        }

        public async Task<BusinessEditViewModel?> BuildEditViewModelAsync(int id, string? userId, bool isAdmin)
        {
            var business = await _businessRepository.GetWithDetailsByIdAsync(id);

            if (business is null || !CanManageBusiness(business, userId, isAdmin))
            {
                return null;
            }

            var model = new BusinessEditViewModel
            {
                Id = business.Id,
                Name = business.Name,
                CategoryId = business.CategoryId,
                DistrictId = business.DistrictId,
                NeighborhoodId = business.NeighborhoodId,
                Description = business.Description,
                ShortDescription = business.ShortDescription,
                Phone = business.Phone,
                WhatsApp = business.WhatsApp,
                Email = business.Email,
                WebsiteUrl = business.WebsiteUrl,
                Address = business.Address,
                Latitude = business.Latitude,
                Longitude = business.Longitude,
                ExistingLogoUrl = business.LogoUrl,
                ExistingCoverImageUrl = business.CoverImageUrl,
                Categories = await GetCategorySelectListAsync(business.CategoryId),
                Districts = await GetDistrictSelectListAsync(business.DistrictId),
                Neighborhoods = await GetNeighborhoodSelectListAsync(business.NeighborhoodId)
            };

            return model;
        }

        public async Task<int> CreateBusinessAsync(BusinessCreateViewModel model, string? ownerId, bool isAdmin)
        {
            var bolu = await _context.Cities.FirstAsync(c => c.Slug == "bolu");
            var logoUrl = await _fileUploadService.UploadAsync(model.LogoFile, "businesses/logos");
            var coverUrl = await _fileUploadService.UploadAsync(model.CoverFile, "businesses/covers");
            var slug = await _slugService.GenerateUniqueSlugAsync(model.Name, s => _context.Businesses.AnyAsync(b => b.Slug == s));
            var now = DateTime.UtcNow;

            var business = new Business
            {
                OwnerId = ownerId,
                CategoryId = model.CategoryId,
                CityId = bolu.Id,
                DistrictId = model.DistrictId,
                NeighborhoodId = model.NeighborhoodId,
                Name = model.Name,
                Slug = slug,
                Description = model.Description,
                ShortDescription = model.ShortDescription,
                Phone = model.Phone,
                WhatsApp = model.WhatsApp,
                Email = model.Email,
                WebsiteUrl = model.WebsiteUrl,
                Address = model.Address,
                Latitude = model.Latitude,
                Longitude = model.Longitude,
                LogoUrl = logoUrl,
                CoverImageUrl = coverUrl,
                IsApproved = isAdmin,
                IsActive = true,
                IsFeatured = false,
                CreatedAt = now,
                ApprovedAt = isAdmin ? now : null,
                Stats = new BusinessStat()
            };

            await _businessRepository.AddAsync(business);
            await _businessRepository.SaveChangesAsync();

            return business.Id;
        }

        public async Task<bool> UpdateBusinessAsync(BusinessEditViewModel model, string? userId, bool isAdmin)
        {
            var business = await _businessRepository.GetWithDetailsByIdAsync(model.Id);

            if (business is null || !CanManageBusiness(business, userId, isAdmin))
            {
                return false;
            }

            business.CategoryId = model.CategoryId;
            business.DistrictId = model.DistrictId;
            business.NeighborhoodId = model.NeighborhoodId;
            business.Name = model.Name;
            business.Description = model.Description;
            business.ShortDescription = model.ShortDescription;
            business.Phone = model.Phone;
            business.WhatsApp = model.WhatsApp;
            business.Email = model.Email;
            business.WebsiteUrl = model.WebsiteUrl;
            business.Address = model.Address;
            business.Latitude = model.Latitude;
            business.Longitude = model.Longitude;
            business.UpdatedAt = DateTime.UtcNow;

            var logoUrl = await _fileUploadService.UploadAsync(model.LogoFile, "businesses/logos");
            if (logoUrl is not null)
            {
                await _fileUploadService.DeleteAsync(business.LogoUrl);
                business.LogoUrl = logoUrl;
            }

            var coverUrl = await _fileUploadService.UploadAsync(model.CoverFile, "businesses/covers");
            if (coverUrl is not null)
            {
                await _fileUploadService.DeleteAsync(business.CoverImageUrl);
                business.CoverImageUrl = coverUrl;
            }

            await _businessRepository.SaveChangesAsync();
            return true;
        }

        public async Task<bool> ApproveBusinessAsync(int id)
        {
            var business = await _context.Businesses.FindAsync(id);

            if (business is null)
            {
                return false;
            }

            business.IsApproved = true;
            business.ApprovedAt = DateTime.UtcNow;
            business.UpdatedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();

            return true;
        }

        public async Task<bool> ToggleFeaturedAsync(int id)
        {
            var business = await _context.Businesses.FindAsync(id);

            if (business is null)
            {
                return false;
            }

            business.IsFeatured = !business.IsFeatured;
            business.UpdatedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();

            return true;
        }

        public async Task<bool> ToggleActiveAsync(int id)
        {
            var business = await _context.Businesses.FindAsync(id);

            if (business is null)
            {
                return false;
            }

            business.IsActive = !business.IsActive;
            business.UpdatedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();

            return true;
        }

        public Task<Business?> GetBusinessForContactAsync(int id)
        {
            return _context.Businesses
                .AsNoTracking()
                .FirstOrDefaultAsync(b => b.Id == id && b.IsApproved && b.IsActive);
        }

        public async Task<IReadOnlyList<AdminBusinessRowViewModel>> GetAdminBusinessesAsync(string? status = null)
        {
            var query = _context.Businesses
                .AsNoTracking()
                .Include(b => b.Category)
                .Include(b => b.District)
                .AsQueryable();

            query = status switch
            {
                "approved" => query.Where(b => b.IsApproved && b.IsActive),
                "pending" => query.Where(b => !b.IsApproved),
                "passive" => query.Where(b => !b.IsActive),
                "featured" => query.Where(b => b.IsFeatured),
                _ => query
            };

            return await query
                .OrderByDescending(b => b.CreatedAt)
                .Select(b => new AdminBusinessRowViewModel
                {
                    Id = b.Id,
                    Name = b.Name,
                    Slug = b.Slug,
                    CategoryName = b.Category.Name,
                    DistrictName = b.District.Name,
                    IsApproved = b.IsApproved,
                    IsActive = b.IsActive,
                    IsFeatured = b.IsFeatured,
                    CreatedAt = b.CreatedAt
                })
                .ToListAsync();
        }

        public async Task<IReadOnlyList<OwnerBusinessRowViewModel>> GetOwnerBusinessesAsync(string userId, bool isAdmin)
        {
            var query = _context.Businesses
                .AsNoTracking()
                .Include(b => b.Category)
                .Include(b => b.Stats)
                .AsQueryable();

            if (!isAdmin)
            {
                query = query.Where(b => b.OwnerId == userId);
            }

            return await query
                .OrderByDescending(b => b.CreatedAt)
                .Select(b => new OwnerBusinessRowViewModel
                {
                    Id = b.Id,
                    Name = b.Name,
                    CategoryName = b.Category.Name,
                    IsApproved = b.IsApproved,
                    IsActive = b.IsActive,
                    ViewCount = b.Stats == null ? 0 : b.Stats.ViewCount,
                    PhoneClickCount = b.Stats == null ? 0 : b.Stats.PhoneClickCount,
                    WhatsAppClickCount = b.Stats == null ? 0 : b.Stats.WhatsAppClickCount,
                    DirectionClickCount = b.Stats == null ? 0 : b.Stats.DirectionClickCount
                })
                .ToListAsync();
        }

        public async Task<OwnerDashboardViewModel> GetOwnerDashboardAsync(string userId, bool isAdmin)
        {
            var businesses = await GetOwnerBusinessesAsync(userId, isAdmin);

            return new OwnerDashboardViewModel
            {
                TotalBusinesses = businesses.Count,
                ApprovedBusinesses = businesses.Count(b => b.IsApproved),
                PendingBusinesses = businesses.Count(b => !b.IsApproved),
                TotalViews = businesses.Sum(b => b.ViewCount),
                PhoneClicks = businesses.Sum(b => b.PhoneClickCount),
                WhatsAppClicks = businesses.Sum(b => b.WhatsAppClickCount),
                DirectionClicks = businesses.Sum(b => b.DirectionClickCount),
                Businesses = businesses
            };
        }

        public async Task<BusinessImageManageViewModel?> GetImageManageViewModelAsync(int businessId, string? userId, bool isAdmin)
        {
            var business = await _businessRepository.GetWithDetailsByIdAsync(businessId);

            if (business is null || !CanManageBusiness(business, userId, isAdmin))
            {
                return null;
            }

            return new BusinessImageManageViewModel
            {
                BusinessId = business.Id,
                BusinessName = business.Name,
                Images = business.Images
                    .OrderBy(i => i.DisplayOrder)
                    .Select(i => new BusinessImageViewModel
                    {
                        Id = i.Id,
                        ImageUrl = i.ImageUrl,
                        AltText = i.AltText
                    })
                    .ToList()
            };
        }

        public async Task<bool> AddGalleryImageAsync(int businessId, IFormFile? file, string? altText, string? userId, bool isAdmin)
        {
            var business = await _businessRepository.GetWithDetailsByIdAsync(businessId);

            if (business is null || !CanManageBusiness(business, userId, isAdmin))
            {
                return false;
            }

            var imageUrl = await _fileUploadService.UploadAsync(file, "businesses/gallery");

            if (imageUrl is null)
            {
                return false;
            }

            _context.BusinessImages.Add(new BusinessImage
            {
                BusinessId = businessId,
                ImageUrl = imageUrl,
                AltText = altText,
                DisplayOrder = business.Images.Count + 1,
                CreatedAt = DateTime.UtcNow
            });

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteGalleryImageAsync(int imageId, string? userId, bool isAdmin)
        {
            var image = await _context.BusinessImages
                .Include(i => i.Business)
                .FirstOrDefaultAsync(i => i.Id == imageId);

            if (image is null || !CanManageBusiness(image.Business, userId, isAdmin))
            {
                return false;
            }

            await _fileUploadService.DeleteAsync(image.ImageUrl);
            _context.BusinessImages.Remove(image);
            await _context.SaveChangesAsync();

            return true;
        }

        public async Task<BusinessHoursViewModel?> GetHoursViewModelAsync(int businessId, string? userId, bool isAdmin)
        {
            var business = await _businessRepository.GetWithDetailsByIdAsync(businessId);

            if (business is null || !CanManageBusiness(business, userId, isAdmin))
            {
                return null;
            }

            var hours = Enum.GetValues<DayOfWeek>()
                .Select(day =>
                {
                    var existing = business.Hours.FirstOrDefault(h => h.DayOfWeek == day);

                    return new BusinessHourEditItemViewModel
                    {
                        DayOfWeek = day,
                        OpenTime = existing?.OpenTime ?? new TimeSpan(9, 0, 0),
                        CloseTime = existing?.CloseTime ?? new TimeSpan(18, 0, 0),
                        IsClosed = existing?.IsClosed ?? false
                    };
                })
                .ToList();

            return new BusinessHoursViewModel
            {
                BusinessId = business.Id,
                BusinessName = business.Name,
                Hours = hours
            };
        }

        public async Task<bool> UpdateHoursAsync(BusinessHoursViewModel model, string? userId, bool isAdmin)
        {
            var business = await _businessRepository.GetWithDetailsByIdAsync(model.BusinessId);

            if (business is null || !CanManageBusiness(business, userId, isAdmin))
            {
                return false;
            }

            foreach (var item in model.Hours)
            {
                var hour = business.Hours.FirstOrDefault(h => h.DayOfWeek == item.DayOfWeek);

                if (hour is null)
                {
                    hour = new BusinessHour
                    {
                        BusinessId = business.Id,
                        DayOfWeek = item.DayOfWeek
                    };

                    _context.BusinessHours.Add(hour);
                }

                hour.OpenTime = item.IsClosed ? null : item.OpenTime;
                hour.CloseTime = item.IsClosed ? null : item.CloseTime;
                hour.IsClosed = item.IsClosed;
            }

            business.UpdatedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();

            return true;
        }

        private async Task<IReadOnlyList<DistrictFilterViewModel>> GetDistrictFiltersAsync()
        {
            return await _context.Districts
                .AsNoTracking()
                .OrderBy(d => d.Name)
                .Select(d => new DistrictFilterViewModel
                {
                    Id = d.Id,
                    Name = d.Name
                })
                .ToListAsync();
        }

        private async Task<IReadOnlyList<SelectListItem>> GetCategorySelectListAsync(int? selectedId)
        {
            return await _context.Categories
                .AsNoTracking()
                .Where(c => c.IsActive)
                .OrderBy(c => c.DisplayOrder)
                .Select(c => new SelectListItem
                {
                    Value = c.Id.ToString(),
                    Text = c.Name,
                    Selected = selectedId.HasValue && c.Id == selectedId.Value
                })
                .ToListAsync();
        }

        private async Task<IReadOnlyList<SelectListItem>> GetDistrictSelectListAsync(int? selectedId)
        {
            return await _context.Districts
                .AsNoTracking()
                .OrderBy(d => d.Name)
                .Select(d => new SelectListItem
                {
                    Value = d.Id.ToString(),
                    Text = d.Name,
                    Selected = selectedId.HasValue && d.Id == selectedId.Value
                })
                .ToListAsync();
        }

        private async Task<IReadOnlyList<SelectListItem>> GetNeighborhoodSelectListAsync(int? selectedId)
        {
            return await _context.Neighborhoods
                .AsNoTracking()
                .OrderBy(n => n.Name)
                .Select(n => new SelectListItem
                {
                    Value = n.Id.ToString(),
                    Text = n.Name,
                    Selected = selectedId.HasValue && n.Id == selectedId.Value
                })
                .ToListAsync();
        }

        private static bool CanManageBusiness(Business business, string? userId, bool isAdmin)
        {
            return isAdmin || (!string.IsNullOrWhiteSpace(userId) && business.OwnerId == userId);
        }

        private static BusinessCardViewModel ToCardProjection(Business business)
        {
            return new BusinessCardViewModel
            {
                Id = business.Id,
                Name = business.Name,
                Slug = business.Slug,
                CategoryName = business.Category.Name,
                DistrictName = business.District.Name,
                ShortDescription = business.ShortDescription,
                LogoUrl = business.LogoUrl,
                Phone = business.Phone,
                WhatsApp = business.WhatsApp,
                AverageRating = business.AverageRating,
                ReviewCount = business.ReviewCount,
                IsFeatured = business.IsFeatured,
                Address = business.Address
            };
        }
    }
}
