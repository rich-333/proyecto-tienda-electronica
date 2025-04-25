using Microsoft.AspNetCore.Mvc;

namespace tienda_electronica.Controllers
{
    public class CategoriasController : Controller
    {
        public IActionResult Gestion()
        {
            return View();
        }

        public IActionResult Agregar()
        {
            return View();
        }
    }
}
