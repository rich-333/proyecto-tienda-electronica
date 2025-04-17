using Microsoft.AspNetCore.Mvc;

namespace tienda_electronica.Controllers
{
    public class ProductosController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
