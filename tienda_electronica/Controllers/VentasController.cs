using Microsoft.AspNetCore.Mvc;

namespace tienda_electronica.Controllers
{
    public class VentasController : Controller
    {
        public IActionResult Gestion()
        {
            return View();
        }
    }
}
