using Microsoft.AspNetCore.Mvc;
using tienda_electronica.Data;

namespace tienda_electronica.Controllers
{
    public class FavoritosController : Controller
    {
        private readonly FavoritoData favoritoData;

        public FavoritosController (IConfiguration config)
        {
            favoritoData = new FavoritoData(config);
        }
        public IActionResult Index()
        {
            var idCliente = HttpContext.Session.GetInt32("idCliente");

            if (idCliente == null)
                return RedirectToAction("Login", "Cuenta");

            var favoritos = favoritoData.ObtenerFavoritosPorCliente(idCliente.Value);
            return View(favoritos);
        }

        [HttpPost]
        public IActionResult Agregar (int idProducto)
        {
            int? idCliente = HttpContext.Session.GetInt32("idCliente");
            if (idCliente == null)
                return RedirectToAction("Login", "Cuenta");

            favoritoData.AgregarFavorito(idCliente.Value, idProducto);
            return Redirect(Request.Headers["Referer"].ToString());
        }

        public IActionResult Eliminar(int id)
        {
            try
            {
                favoritoData.EliminarFavorito(id);
                TempData["Mensaje Eliminar"] = "Favorito eliminado correctamente.";
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;
                return RedirectToAction("Index");
            }
        }
    }
}
