using Microsoft.AspNetCore.Mvc;

namespace tienda_electronica.Controllers
{
    public class FavoritosController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
