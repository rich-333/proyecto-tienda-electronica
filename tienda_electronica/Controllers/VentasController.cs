using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using tienda_electronica.Data;
using tienda_electronica.Models.Ventas;

namespace tienda_electronica.Controllers
{
    public class VentasController : Controller
    {
        private readonly VentaData ventaData;

        public VentasController (IConfiguration configuration)
        {
            ventaData = new VentaData (configuration);
        }
        public IActionResult Gestion()
        {
            var ventas = ventaData.ObtenerVentas();
            return View(ventas);
        }

        [HttpGet]
        public IActionResult Editar(int id)
        {
            var ventas = ventaData.ObtenerVentaPorId(id);

            if (ventas == null)
            {
                TempData["Error"] = "Venta no encontrada,";
                return RedirectToAction("Gestion");
            }

            ViewBag.Estados = new List<SelectListItem>
            {
                new SelectListItem { Text = "Pendiente", Value = "Pendiente" },
                new SelectListItem { Text = "Enviado", Value = "Enviado" },
                new SelectListItem { Text = "Entregado", Value = "Entregado" },
                new SelectListItem { Text = "Cancelado", Value = "Cancelado" },
            };

            return View(ventas);
        }

        [HttpPost]
        public IActionResult Actualizar(Venta venta)
        {
            try
            {
                ventaData.EditarVenta(venta);
                TempData["MensajeAgregar"] = "Venta editada correctamente.";

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
