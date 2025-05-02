using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using tienda_electronica.Data;
using tienda_electronica.Models.Usuarios;

namespace tienda_electronica.Controllers
{
    public class UsuariosController : Controller
    {
        private readonly UsuarioData usuarioData;

        public UsuariosController(IConfiguration config)
        {
            usuarioData = new UsuarioData(config);
        }
        public IActionResult Gestion()
        {
            ViewBag.TituloMenu = "Gestion de Usuarios";

            var usuarios = usuarioData.ObtenerUsuarios();
            return View(usuarios);
        }

        public IActionResult Eliminar(int id)
        {
            try
            {
                usuarioData.EliminarUsuarios(id);
                TempData["MensajeEliminar"] = "Usuario eliminado correctamente.";
                return RedirectToAction("Gestion");
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;
                return RedirectToAction("Gestion");
            }
        }
        public IActionResult Agregar()
        {
            ViewBag.Roles = new List<SelectListItem>
            {
                new SelectListItem { Text = "Administrador", Value = "Administrador" },
                new SelectListItem { Text = "Gestor Productos", Value = "Gestor Productos" },
                new SelectListItem { Text = "Gestor Ventas", Value = "Gestor Ventas" }
            };
            
            return View(new Usuario());
        }

        [HttpPost]
        public IActionResult Guardar(Usuario usuario, IFormFile rutaFotoEmpleado)
        {
            try
            {
                if (usuario.idUsuario == 0)
                {
                    int idUsuario = usuarioData.AgregarUsuario(usuario, rutaFotoEmpleado);
                    TempData["MensajeAgregar"] = "Usuario agregado correctamente.";
                }
                else
                {
                    var usuarioExistente = usuarioData.ObtenerUsuarioPorId(usuario.idUsuario);

                    if (usuarioExistente == null)
                    {
                        TempData["Error"] = "No se encontró el usuario para editar.";
                        return RedirectToAction("Gestion");
                    }

                    if (rutaFotoEmpleado == null)
                    {
                        usuario.rutaFotoEmpleado = usuarioExistente.rutaFotoEmpleado;
                    }
                    usuarioData.EditarUsuario(usuario, rutaFotoEmpleado);
                    TempData["MensajeAgregar"] = "Usuario editado correctamente.";
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
            var usuario = usuarioData.ObtenerUsuarioPorId(id);

            if (usuario == null)
            {
                TempData["Error"] = "Usuario no encontrado";
                return RedirectToAction("Gestion");
            }

            ViewBag.Roles = new List<SelectListItem>
            {
                new SelectListItem { Text = "Administrador", Value = "Administrador" },
                new SelectListItem { Text = "Gestor Productos", Value = "Gestor Productos" },
                new SelectListItem { Text = "Gestor Ventas", Value = "Gestor Ventas" }
            };

            return View("Agregar", usuario);
        }
    }
}
