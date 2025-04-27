using Microsoft.AspNetCore.Mvc;
using tienda_electronica.Data;
using tienda_electronica.Models;
using tienda_electronica.Models.Productos;

namespace tienda_electronica.Controllers
{
    public class ProductosController : Controller
    {
        private readonly ProductoData productoData;
        private readonly CategoriaData categoriaData;
        public ProductosController(IConfiguration config)
        {
            productoData = new ProductoData(config);
            categoriaData = new CategoriaData(config);
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
            try
            {
                if (productoData.ProductoTienePedidos(id))
                {
                    TempData["Error"] = "No se puede eliminar el producto porque está relacionado con pedidos.";
                    return RedirectToAction("Gestion"); 
                }
                else
                {
                    productoData.EliminarProducto(id);
                    TempData["MensajeEliminar"] = "Producto eliminado correctamente.";
                    return RedirectToAction("Gestion");
                }
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;
                return RedirectToAction("Gestion");
            }
        }

        public IActionResult Agregar() 
        {
            var categorias = categoriaData.ObtenerCategorias();
            ViewBag.Categorias = categorias;
            return View();
        }

        [HttpPost]
        public IActionResult Agregar(Producto producto, IFormFile imagenPrincipal, List<IFormFile> imagenesAdicionales)
        {
            try
            {
                int idProducto = productoData.AgregarProducto(producto, imagenPrincipal, imagenesAdicionales);
                TempData["MensajeAgregar"] = "Producto agregado correctamente.";
                return RedirectToAction("Gestion");
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;
                return RedirectToAction("Agregar");
            }
        }
    }
}
