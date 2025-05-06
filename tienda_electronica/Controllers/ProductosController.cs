using Microsoft.AspNetCore.Mvc;
using tienda_electronica.Data;
using tienda_electronica.Filters;
using tienda_electronica.Models;
using tienda_electronica.Models.Categorias;
using tienda_electronica.Models.Productos;

namespace tienda_electronica.Controllers
{
    public class ProductosController : Controller
    {
        private readonly ProductoData productoData;
        private readonly CategoriaData categoriaData;
        private readonly FavoritoData favoritoData;

        public ProductosController(IConfiguration config)
        {
            productoData = new ProductoData(config);
            categoriaData = new CategoriaData(config);
            favoritoData = new FavoritoData(config);
        }
        public IActionResult Index(int? categoriaId, string busqueda)
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

            if (!string.IsNullOrEmpty(busqueda))
            {
                productos = productos
                    .Where(p => p.nombre.Contains(busqueda, StringComparison.OrdinalIgnoreCase) || 
                            p.descripcion.Contains(busqueda, StringComparison.OrdinalIgnoreCase))
                    .ToList();      
            }

            var categorias = categoriaData.ObtenerCategorias();
            ViewBag.Categorias = categorias;
            ViewBag.CategoriaSeleccionadaId = categoriaId;
            ViewBag.BusquedaActual = busqueda;

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

        [AuthorizeRol("Administrador", "Gestor Productos")]
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
