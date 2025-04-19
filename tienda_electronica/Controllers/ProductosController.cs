using Microsoft.AspNetCore.Mvc;

namespace tienda_electronica.Controllers
{
    public class ProductosController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
        public IActionResult Detalle(int id)
        {
            // Aquí podrías traer el producto desde una base de datos
            // Por ahora solo pasamos el ID a la vista como ejemplo
            ViewBag.ProductoId = id;
            return View();
        }
        public IActionResult Gestion()
        {
            return View();
        }
    }
}
