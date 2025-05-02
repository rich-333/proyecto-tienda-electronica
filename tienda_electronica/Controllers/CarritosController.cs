using Microsoft.AspNetCore.Mvc;

namespace tienda_electronica.Controllers
{
    public class CarritosController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
