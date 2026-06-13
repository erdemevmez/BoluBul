using BoluBul.Data;
using BoluBul.Models;
using BoluBul.Services.Interfaces;
using BoluBul.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;

namespace BoluBul.Controllers
{
    public class HomeController : AppControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly IBusinessService _businessService;
        private readonly ICategoryService _categoryService;

        public HomeController(
            ApplicationDbContext context,
            IBusinessService businessService,
            ICategoryService categoryService)
        {
            _context = context;
            _businessService = businessService;
            _categoryService = categoryService;
        }

        public async Task<IActionResult> Index()
        {
            try
            {
                var model = new HomeViewModel
                {
                    FeaturedBusinesses = await _businessService.GetFeaturedBusinessesAsync(),
                    LatestBusinesses = await _businessService.GetLatestBusinessesAsync(3),
                    Categories = await _categoryService.GetCategoryWithBusinessCountAsync(),
                    RegisteredBusinessCount = await _context.Businesses.CountAsync(b => b.IsApproved && b.IsActive),
                    CategoryCount = await _context.Categories.CountAsync(c => c.IsActive),
                    ReviewCount = await _context.Reviews.CountAsync(r => r.IsApproved),
                    FeaturedBusinessCount = await _context.Businesses.CountAsync(b => b.IsApproved && b.IsActive && b.IsFeatured),
                    DiscoveryCards = new List<DiscoveryCardViewModel>
                    {
                        new() { Title = "Abant hissi", Text = "Göl, orman ve sakin hafta sonu rotalarına yakın işletmeleri keşfet.", CssClass = "discovery-abant" },
                        new() { Title = "Yedigöller ruhu", Text = "Doğayla iç içe mekânları, kahvaltı noktalarını ve aile duraklarını bul.", CssClass = "discovery-yedigoller" },
                        new() { Title = "Mengen lezzeti", Text = "Bolu’nun mutfak kültürüyle uyumlu restoran ve yerel tatları incele.", CssClass = "discovery-mengen" },
                        new() { Title = "Kartalkaya enerjisi", Text = "Konaklama, spor ve kış rotalarına destek olan işletmeleri gör.", CssClass = "discovery-kartalkaya" }
                    }
                };

                return View(model);
            }
            catch (Exception ex) when (IsDatabaseException(ex))
            {
                return View(new HomeViewModel());
            }
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
