using Microsoft.AspNetCore.Mvc;
using tienda_electronica.Data;
using tienda_electronica.Models;
using tienda_electronica.Models.Categorias;
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
        public IActionResult Index(int? categoriaId)
        {
            List<Producto> productos;

            if (categoriaId.HasValue)
            {
                productos = productoData.ObtenerPorCategoria(categoriaId.Value);
            }
            else
            {
                productos = productoData.ObtenerProductos();
            }
            var categorias = categoriaData.ObtenerCategorias();
            ViewBag.Categorias = categorias;

            /*var productos = (categoriaId == null)
                ? productoData.ObtenerProductos()
                : productoData.ObtenerPorCategoria(categoriaId.Value);*/
            return View(productos);
        }
        public IActionResult Detalle(int id)
        {
            var producto = productoData.ObtenerProductoPorId(id);
            if (producto == null) 
            {
                return NotFound();
            }
            //ViewBag.ProductoId = id;
            return View(producto);
        }
        public IActionResult Gestion()
        {
            ViewBag.TituloMenu = "Gestion de Productos";

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
            return View(new Producto());
        }

        [HttpPost]
        public IActionResult Guardar(Producto producto, IFormFile imagenPrincipal, List<IFormFile> imagenesAdicionales)
        {
            try
            {
                /*int idProducto = productoData.AgregarProducto(producto, imagenPrincipal, imagenesAdicionales);
                TempData["MensajeAgregar"] = "Producto agregado correctamente.";
                return RedirectToAction("Gestion");*/
                if (producto.idProducto == 0)
                {
                    int idProducto = productoData.AgregarProducto(producto, imagenPrincipal, imagenesAdicionales);
                    TempData["MensajeAgregar"] = "Producto agregado correctamente.";
                }
                else
                {
                    productoData.EditarProducto(producto, imagenPrincipal, imagenesAdicionales);
                    TempData["MensajeAgregar"] = "Producto editado correctamente.";
                }

                return RedirectToAction("Gestion");
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;
                return RedirectToAction("Agregar");
            }
        }

        [HttpGet]
        public IActionResult Editar(int id)
        {
            var producto = productoData.ObtenerProductoPorId(id);
            if (producto == null)
            {
                TempData["Error"] = "Producto no encontrado";
                return RedirectToAction("Gestion");
            }

            var categorias = categoriaData.ObtenerCategorias();
            ViewBag.Categorias = categorias;

            return View("Agregar", producto);
        }

    }
}
