using Microsoft.AspNetCore.Mvc;
using tienda_electronica.Data;

namespace tienda_electronica.Controllers
{
    public class CuentaController : Controller
    {
        private readonly CuentaData cuentaData;

        public CuentaController (IConfiguration config)
        {
            cuentaData = new CuentaData(config);
        }

        [HttpPost]
        public IActionResult Login(string email, string contrasena)
        {
            var usuario = cuentaData.AutenticarUsuario(email, contrasena);
            if (usuario != null) 
            {
                switch (usuario.rol) 
                {
                    case "Administrador":
                    case "Gestor Productos":
                        return RedirectToAction("Gestion", "Productos");
                    case "Gestor Ventas":
                        return RedirectToAction("Gestion", "Ventas");
                }
            }
            
            var cliente = cuentaData.AutenticarCliente(email, contrasena);
            if (cliente != null) 
            {
                return RedirectToAction("Index", "Home");
            }

            TempData["Error"] = "Credenciales Incorrectas.";
            return RedirectToAction("Login");
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }
    }
}
