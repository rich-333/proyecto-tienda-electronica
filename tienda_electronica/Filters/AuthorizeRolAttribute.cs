using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace tienda_electronica.Filters
{
    public class AuthorizeRolAttribute : ActionFilterAttribute
    {
        private readonly string[] _roles;

        public AuthorizeRolAttribute(params string[] roles)
        {
            _roles = roles;
        }

        public override void OnActionExecuting(ActionExecutingContext context)
        {
            var rolUsuario = context.HttpContext.Session.GetString("RolUsuario");

            if (string.IsNullOrEmpty(rolUsuario) || !_roles.Contains(rolUsuario))
            {
                context.Result = new RedirectToActionResult("Login", "Cuenta", null);
            }

            base.OnActionExecuting(context);
        }
    }
}
