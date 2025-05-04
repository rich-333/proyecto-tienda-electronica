using Microsoft.AspNetCore.Mvc;
using tienda_electronica.Data;
using tienda_electronica.Models.Productos;

namespace tienda_electronica.Controllers
{
    public class CarritosController : Controller
    {
        private readonly CarritoData carritoData;

        public CarritosController (IConfiguration conifig)
        {
            carritoData = new CarritoData(conifig);
        }
        
        public IActionResult Index()
        {
            var idCliente = HttpContext.Session.GetInt32("idCliente");
            
            if (idCliente == null)
                return RedirectToAction("Login", "Cuenta");

            var productos = carritoData.ObtenerCarritoPorCliente(idCliente.Value);
            return View(productos);
        }

        [HttpPost]
        public IActionResult AgregarAlCarrito(int idProducto, int cantidad)
        {
            int? idCliente = HttpContext.Session.GetInt32("idCliente");
            if (idCliente == null)
                return RedirectToAction("Login", "Cuenta");

            carritoData.AgregarProductoAlCarrito(idCliente.Value, idProducto, cantidad);
            return RedirectToAction("Index", "Carritos");
        }

        public IActionResult Eliminar(int id) 
        {
            try
            {
                carritoData.EliminarProductoDelCarrito(id);
                TempData["Mensaje Eliminar"] = "Producto eliminado del carrito correctamente.";
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
