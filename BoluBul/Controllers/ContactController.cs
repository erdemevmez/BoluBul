using BoluBul.Data;
using BoluBul.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BoluBul.Controllers
{
    [AllowAnonymous]
    public class ContactController : AppControllerBase
    {
        private readonly ApplicationDbContext _context;

        public ContactController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public IActionResult Index()
        {
            return Content("İletişim formu endpointi hazır. View sonraki aşamada eklenecek.");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Send(ContactMessage contactMessage)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                contactMessage.CreatedAt = DateTime.UtcNow;
                _context.ContactMessages.Add(contactMessage);
                await _context.SaveChangesAsync();

                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex) when (IsDatabaseException(ex))
            {
                return DatabaseUnavailable(ex);
            }
        }
    }
}
