using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using tienda_electronica.Data;
using tienda_electronica.Filters;
using tienda_electronica.Models.Usuarios;

namespace tienda_electronica.Controllers
{
    [AuthorizeRol("Administrador")]
    public class ClientesController : Controller
    {
        private readonly ClienteData clienteData;
       
        public ClientesController (IConfiguration config)
        {
            clienteData = new ClienteData (config);
        }
        public IActionResult Gestion()
        {
            ViewBag.TituloMenu = "Gestion de Clientes";
            
            var clientes = clienteData.ObtenerClientes();
            return View(clientes);
        }

        [HttpGet]
        public IActionResult Editar(int id)
        {
            var clientes = clienteData.ObtenerClientePorId(id);
            if (clientes == null)
            {
                TempData["Error"] = "Cliente no encontrado.";
                return RedirectToAction("Gestion");
            }

            return View("Editar", clientes);
        }

        [HttpPost]
        public IActionResult Actualizar (Cliente cliente)
        {
            try
            {
                clienteData.EditarCliente(cliente);
                TempData["Mensaje Agregar"] = "Cliente editado correctamente.";

                return RedirectToAction("Gestion");
            }
            catch (Exception ex) 
            {
                TempData["Error"] = ex.Message;
                return RedirectToAction("Editar");
            }
        }
    }
}
