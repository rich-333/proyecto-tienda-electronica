using Microsoft.AspNetCore.Mvc;

namespace tienda_electronica.Controllers
{
    public class CuentaController : Controller
    {
        public IActionResult Login()
        {
            return View();
        }
    }
}
