using Microsoft.AspNetCore.Mvc;
using tienda_electronica.Data;
using tienda_electronica.Models;

namespace tienda_electronica.Controllers
{
    public class ProductosController : Controller
    {
        private readonly ProductoData productoData;

        public ProductosController(IConfiguration config)
        {
            productoData = new ProductoData(config);
        }
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
            var productos = productoData.ObtenerProductos();
            return View(productos);
        }

        public IActionResult Eliminar(int id)
        {
            productoData.EliminarProducto(id);
            return RedirectToAction("Gestion");
        }

        public ActionResult Agregar() 
        {
            return View();
        }
    }
}
