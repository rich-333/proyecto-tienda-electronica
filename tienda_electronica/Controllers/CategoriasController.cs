using Microsoft.AspNetCore.Mvc;
using tienda_electronica.Data;
using tienda_electronica.Filters;
using tienda_electronica.Models.Categorias;

namespace tienda_electronica.Controllers
{
    [AuthorizeRol("Administrador", "Gestor Productos")]
    public class CategoriasController : Controller
    {
        public readonly CategoriaData categoriaData;

        public CategoriasController (IConfiguration config)
        {
            categoriaData = new CategoriaData (config);
        }
        public IActionResult Gestion()
        {
            ViewBag.TituloMenu = "Gestion de Categorias";

            var categoria = categoriaData.ObtenerCategorias();
            return View(categoria);
        }

        public IActionResult Agregar()
        {
            return View(new Categoria());
        }

        public IActionResult Eliminar(int id)
        {
            try
            {
                categoriaData.EliminarCategoria(id);
                TempData["Mensaje Eliminar"] = "Categoria eliminada correctamente";
                return RedirectToAction("Gestion");
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;
                return RedirectToAction("Gestion");
            }
        }

        [HttpPost]
        public IActionResult Guardar (Categoria categoria)
        {
            try
            {
                if (categoria.idCategoria == 0)
                {
                    int idProducto = categoriaData.AgregarCategoria(categoria);
                    TempData["MensajeAgregar"] = "Categoria agregada correctamente.";
                }
                else
                {
                    categoriaData.EditarCategoria(categoria);
                    TempData["MensajeAgregar"] = "Categoria editada correctamente.";
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
            var categoria = categoriaData.ObtenerCategoriaPorId(id);
            if (categoria == null)
            {
                TempData["Error"] = "Categoria no encontrada";
                return RedirectToAction("Gestion");
            }
            return View("Agregar", categoria);
        }
    }
}
