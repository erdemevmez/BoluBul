using System.Data.Common;
using Microsoft.AspNetCore.Mvc;

namespace BoluBul.Controllers
{
    public abstract class AppControllerBase : Controller
    {
        protected IActionResult DatabaseUnavailable(Exception exception)
        {
            Response.StatusCode = StatusCodes.Status503ServiceUnavailable;
            return Content("Veritabanı henüz hazır değil. PostgreSQL bağlantısı ve EF migration tamamlandıktan sonra bu ekran çalışacaktır.");
        }

        protected static bool IsDatabaseException(Exception exception)
        {
            for (var current = exception; current is not null; current = current.InnerException)
            {
                if (current is DbException)
                {
                    return true;
                }

                if (current.GetType().Namespace?.StartsWith("Npgsql", StringComparison.Ordinal) == true)
                {
                    return true;
                }

                if (current is InvalidOperationException &&
                    current.Message.Contains("transient failure", StringComparison.OrdinalIgnoreCase))
                {
                    return true;
                }
            }

            return false;
        }
    }
}
