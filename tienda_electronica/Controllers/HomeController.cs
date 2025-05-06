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
        private readonly FavoritoData favoritoData;

        public HomeController(ILogger<HomeController> logger, IConfiguration configuration)
        {
            _logger = logger;
            productoData = new ProductoData(configuration);
            categoriaData = new CategoriaData(configuration);
            favoritoData = new FavoritoData(configuration);
        }

        public IActionResult Index()
        {
            var productos = productoData.ObtenerProductos();
            var categorias = categoriaData.ObtenerCategorias();
            ViewBag.Categorias = categorias;

            var idCliente = HttpContext.Session.GetInt32("idCliente");
            var favoritosIds = new List<int>();
            if (idCliente != null)
            {
                var favoritos = favoritoData.ObtenerFavoritosPorCliente(idCliente.Value);
                favoritosIds = favoritos.Select(f => f.idProducto).ToList();
            }

            ViewBag.FavoritosIds = favoritosIds;

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
