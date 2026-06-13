using BoluBul.Controllers;
using BoluBul.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BoluBul.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class CategoriesController : AppControllerBase
    {
        private readonly ICategoryService _categoryService;

        public CategoriesController(ICategoryService categoryService)
        {
            _categoryService = categoryService;
        }

        public async Task<IActionResult> Index()
        {
            try
            {
                return View(await _categoryService.GetAdminCategoriesAsync());
            }
            catch (Exception ex) when (IsDatabaseException(ex))
            {
                return DatabaseUnavailable(ex);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(string name, string? icon, int displayOrder = 100)
        {
            if (!string.IsNullOrWhiteSpace(name))
            {
                await _categoryService.CreateCategoryAsync(name, icon, displayOrder);
                TempData["Success"] = "Kategori başarıyla oluşturuldu.";
            }
            else
            {
                TempData["Error"] = "Kategori adı boş bırakılamaz.";
            }

            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Update(int id, string name, string? icon, int displayOrder, bool isActive)
        {
            TempData["Success"] = await _categoryService.UpdateCategoryAsync(id, name, icon, displayOrder, isActive)
                ? "Kategori bilgileri güncellendi."
                : "Kategori bulunamadı.";
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ToggleActive(int id)
        {
            TempData["Success"] = await _categoryService.ToggleActiveAsync(id)
                ? "Kategori yayın durumu güncellendi."
                : "Kategori bulunamadı.";
            return RedirectToAction(nameof(Index));
        }
    }
}
