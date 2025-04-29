using Microsoft.AspNetCore.Mvc;
using tienda_electronica.Data;

namespace tienda_electronica.Controllers
{
    public class CategoriasController : Controller
    {
        public readonly CategoriaData categoriaData;

        public CategoriasController (IConfiguration config)
        {
            categoriaData = new CategoriaData (config);
        }
        public IActionResult Gestion()
        {
            var categoria = categoriaData.ObtenerCategorias();
            return View(categoria);
        }

        public IActionResult Agregar()
        {
            return View();
        }
    }
}
