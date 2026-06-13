using BoluBul.Models;
using BoluBul.Services.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.DependencyInjection;

namespace BoluBul.Data
{
    public static class SeedData
    {
        private const string AdminRole = "Admin";
        private const string BusinessOwnerRole = "BusinessOwner";
        private const string UserRole = "User";

        public static async Task InitializeAsync(IServiceProvider serviceProvider)
        {
            var context = serviceProvider.GetRequiredService<ApplicationDbContext>();

            if (!await context.Database.CanConnectAsync())
            {
                return;
            }

            var databaseCreator = context.Database.GetService<IRelationalDatabaseCreator>();
            if (!await databaseCreator.HasTablesAsync())
            {
                return;
            }

            var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();
            var userManager = serviceProvider.GetRequiredService<UserManager<ApplicationUser>>();
            var slugService = serviceProvider.GetRequiredService<ISlugService>();

            await SeedRolesAsync(roleManager);
            await SeedAdminAsync(userManager);
            await SeedBoluDataAsync(context, slugService);
        }

        private static async Task SeedRolesAsync(RoleManager<IdentityRole> roleManager)
        {
            var roles = new[] { AdminRole, BusinessOwnerRole, UserRole };

            foreach (var role in roles)
            {
                if (!await roleManager.RoleExistsAsync(role))
                {
                    var result = await roleManager.CreateAsync(new IdentityRole(role));
                    ThrowIfFailed(result, $"Role could not be created: {role}");
                }
            }
        }

        private static async Task SeedAdminAsync(UserManager<ApplicationUser> userManager)
        {
            const string adminEmail = "admin@bolubul.com";
            const string adminPassword = "Admin123*";

            var admin = await userManager.FindByEmailAsync(adminEmail);

            if (admin is null)
            {
                admin = new ApplicationUser
                {
                    UserName = adminEmail,
                    Email = adminEmail,
                    EmailConfirmed = true,
                    FullName = "BoluBul Admin",
                    CreatedAt = DateTime.UtcNow
                };

                var createResult = await userManager.CreateAsync(admin, adminPassword);
                ThrowIfFailed(createResult, "Default admin user could not be created.");
            }

            if (!await userManager.IsInRoleAsync(admin, AdminRole))
            {
                var roleResult = await userManager.AddToRoleAsync(admin, AdminRole);
                ThrowIfFailed(roleResult, "Default admin user could not be assigned to Admin role.");
            }
        }

        private static async Task SeedBoluDataAsync(ApplicationDbContext context, ISlugService slugService)
        {
            var bolu = await context.Cities.FirstOrDefaultAsync(c => c.Slug == "bolu");

            if (bolu is null)
            {
                bolu = new City
                {
                    Name = "Bolu",
                    Slug = "bolu",
                    PlateCode = 14
                };

                context.Cities.Add(bolu);
                await context.SaveChangesAsync();
            }

            await SeedDistrictsAsync(context, slugService, bolu);
            await SeedCategoriesAsync(context, slugService);
            await SeedSampleBusinessesAsync(context, slugService, bolu);
        }

        private static async Task SeedDistrictsAsync(ApplicationDbContext context, ISlugService slugService, City bolu)
        {
            var districtNames = new[]
            {
                "Merkez",
                "Gerede",
                "Mudurnu",
                "Mengen",
                "G\u00f6yn\u00fck",
                "Yeni\u00e7a\u011fa",
                "D\u00f6rtdivan",
                "Seben",
                "K\u0131br\u0131sc\u0131k"
            };

            var existingSlugs = await context.Districts
                .Where(d => d.CityId == bolu.Id)
                .Select(d => d.Slug)
                .ToListAsync();

            foreach (var districtName in districtNames)
            {
                var slug = slugService.GenerateSlug(districtName);

                if (!existingSlugs.Contains(slug))
                {
                    context.Districts.Add(new District
                    {
                        CityId = bolu.Id,
                        Name = districtName,
                        Slug = slug
                    });
                }
            }

            await context.SaveChangesAsync();
        }

        private static async Task SeedCategoriesAsync(ApplicationDbContext context, ISlugService slugService)
        {
            var categoryNames = new[]
            {
                "Yiyecek & \u0130\u00e7ecek",
                "Otomotiv",
                "G\u00fczellik & Bak\u0131m",
                "Sa\u011fl\u0131k",
                "E\u011fitim",
                "Giyim & Al\u0131\u015fveri\u015f",
                "Ev Hizmetleri",
                "Konaklama",
                "Spor & Fitness",
                "Teknoloji",
                "Resmi Kurumlar",
                "Di\u011fer"
            };

            var icons = new Dictionary<string, string>
            {
                ["Yiyecek & \u0130\u00e7ecek"] = "YI",
                ["Otomotiv"] = "OT",
                ["G\u00fczellik & Bak\u0131m"] = "GB",
                ["Sa\u011fl\u0131k"] = "SG",
                ["E\u011fitim"] = "EG",
                ["Giyim & Al\u0131\u015fveri\u015f"] = "GA",
                ["Ev Hizmetleri"] = "EV",
                ["Konaklama"] = "KO",
                ["Spor & Fitness"] = "SP",
                ["Teknoloji"] = "TE",
                ["Resmi Kurumlar"] = "RK",
                ["Di\u011fer"] = "DI"
            };

            var existingSlugs = await context.Categories
                .Select(c => c.Slug)
                .ToListAsync();

            for (var index = 0; index < categoryNames.Length; index++)
            {
                var categoryName = categoryNames[index];
                var slug = slugService.GenerateSlug(categoryName);

                if (!existingSlugs.Contains(slug))
                {
                    context.Categories.Add(new Category
                    {
                        Name = categoryName,
                        Slug = slug,
                        Icon = icons[categoryName],
                        DisplayOrder = index + 1,
                        IsActive = true,
                        CreatedAt = DateTime.UtcNow
                    });
                }
            }

            var existingCategories = await context.Categories.ToListAsync();
            foreach (var category in existingCategories)
            {
                if (string.IsNullOrWhiteSpace(category.Icon) && icons.TryGetValue(category.Name, out var icon))
                {
                    category.Icon = icon;
                }
            }

            await context.SaveChangesAsync();
        }

        private static async Task SeedSampleBusinessesAsync(ApplicationDbContext context, ISlugService slugService, City bolu)
        {
            var merkez = await context.Districts
                .FirstAsync(d => d.CityId == bolu.Id && d.Slug == "merkez");

            var categories = await context.Categories.ToDictionaryAsync(c => c.Slug);
            var now = DateTime.UtcNow;

            var sampleBusinesses = new[]
            {
                new Business
                {
                    Name = "\u00d6rnek Restoran",
                    Slug = slugService.GenerateSlug("Bolu Merkez \u00d6rnek Restoran"),
                    CategoryId = categories["yiyecek-icecek"].Id,
                    CityId = bolu.Id,
                    DistrictId = merkez.Id,
                    Description = "BoluBul geli\u015ftirme ortam\u0131 i\u00e7in eklenen \u00f6rnek restoran kayd\u0131.",
                    ShortDescription = "Geli\u015ftirme i\u00e7in \u00f6rnek restoran.",
                    Phone = "03740000001",
                    WhatsApp = "905550000001",
                    Address = "Bolu Merkez",
                    IsApproved = true,
                    IsActive = true,
                    IsFeatured = true,
                    CreatedAt = now,
                    ApprovedAt = now
                },
                new Business
                {
                    Name = "\u00d6rnek Kafe",
                    Slug = slugService.GenerateSlug("Bolu Merkez \u00d6rnek Kafe"),
                    CategoryId = categories["yiyecek-icecek"].Id,
                    CityId = bolu.Id,
                    DistrictId = merkez.Id,
                    Description = "BoluBul geli\u015ftirme ortam\u0131 i\u00e7in eklenen \u00f6rnek kafe kayd\u0131.",
                    ShortDescription = "Geli\u015ftirme i\u00e7in \u00f6rnek kafe.",
                    Phone = "03740000004",
                    WhatsApp = "905550000004",
                    Address = "Bolu Merkez",
                    IsApproved = true,
                    IsActive = true,
                    CreatedAt = now,
                    ApprovedAt = now
                },
                new Business
                {
                    Name = "\u00d6rnek Otel",
                    Slug = slugService.GenerateSlug("Bolu Merkez \u00d6rnek Otel"),
                    CategoryId = categories["konaklama"].Id,
                    CityId = bolu.Id,
                    DistrictId = merkez.Id,
                    Description = "BoluBul geli\u015ftirme ortam\u0131 i\u00e7in eklenen \u00f6rnek otel kayd\u0131.",
                    ShortDescription = "Geli\u015ftirme i\u00e7in \u00f6rnek otel.",
                    Phone = "03740000005",
                    WhatsApp = "905550000005",
                    Address = "Bolu Merkez",
                    IsApproved = true,
                    IsActive = true,
                    IsFeatured = true,
                    CreatedAt = now,
                    ApprovedAt = now
                },
                new Business
                {
                    Name = "\u00d6rnek Kuaf\u00f6r",
                    Slug = slugService.GenerateSlug("Bolu Merkez \u00d6rnek Kuaf\u00f6r"),
                    CategoryId = categories["guzellik-bakim"].Id,
                    CityId = bolu.Id,
                    DistrictId = merkez.Id,
                    Description = "BoluBul geli\u015ftirme ortam\u0131 i\u00e7in eklenen \u00f6rnek kuaf\u00f6r kayd\u0131.",
                    ShortDescription = "Geli\u015ftirme i\u00e7in \u00f6rnek kuaf\u00f6r.",
                    Phone = "03740000002",
                    WhatsApp = "905550000002",
                    Address = "Bolu Merkez",
                    IsApproved = true,
                    IsActive = true,
                    CreatedAt = now,
                    ApprovedAt = now
                },
                new Business
                {
                    Name = "\u00d6rnek Oto Servis",
                    Slug = slugService.GenerateSlug("Bolu Merkez \u00d6rnek Oto Servis"),
                    CategoryId = categories["otomotiv"].Id,
                    CityId = bolu.Id,
                    DistrictId = merkez.Id,
                    Description = "BoluBul geli\u015ftirme ortam\u0131 i\u00e7in eklenen \u00f6rnek oto servis kayd\u0131.",
                    ShortDescription = "Geli\u015ftirme i\u00e7in \u00f6rnek oto servis.",
                    Phone = "03740000003",
                    WhatsApp = "905550000003",
                    Address = "Bolu Merkez",
                    IsApproved = true,
                    IsActive = true,
                    IsFeatured = true,
                    CreatedAt = now,
                    ApprovedAt = now
                }
            };

            foreach (var business in sampleBusinesses)
            {
                if (!await context.Businesses.AnyAsync(b => b.Slug == business.Slug))
                {
                    business.Stats = new BusinessStat();
                    context.Businesses.Add(business);
                }
            }

            await context.SaveChangesAsync();

            var sampleSlugs = sampleBusinesses.Select(b => b.Slug).ToArray();
            var businessesWithoutStats = await context.Businesses
                .Where(b => sampleSlugs.Contains(b.Slug) && b.Stats == null)
                .Select(b => b.Id)
                .ToListAsync();

            foreach (var businessId in businessesWithoutStats)
            {
                context.BusinessStats.Add(new BusinessStat { BusinessId = businessId });
            }

            await context.SaveChangesAsync();
        }

        private static void ThrowIfFailed(IdentityResult result, string message)
        {
            if (result.Succeeded)
            {
                return;
            }

            var errors = string.Join("; ", result.Errors.Select(e => e.Description));
            throw new InvalidOperationException($"{message} {errors}");
        }
    }
}
