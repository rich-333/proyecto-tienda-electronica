using Microsoft.AspNetCore.Mvc;

namespace tienda_electronica.Controllers
{
    public class ClientesController : Controller
    {
        public IActionResult Gestion()
        {
            return View();
        }

        public IActionResult Edit()
        {
            return View();
        }
    }
}
