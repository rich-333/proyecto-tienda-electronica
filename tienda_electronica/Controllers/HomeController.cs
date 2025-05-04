using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using tienda_electronica.Data;
using tienda_electronica.Models;

namespace tienda_electronica.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ProductoData productoData;
        private readonly CategoriaData categoriaData;

        public HomeController(ILogger<HomeController> logger, IConfiguration configuration)
        {
            _logger = logger;
            productoData = new ProductoData(configuration);
            categoriaData = new CategoriaData(configuration);
        }

        public IActionResult Index()
        {
            var productos = productoData.ObtenerProductos();
            var categorias = categoriaData.ObtenerCategorias();
            ViewBag.Categorias = categorias;

            return View(productos);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
