namespace tienda_electronica.Models.Usuarios
{
    public class Persona
    {
        public string nombre { get; set; }
        public string apellido { get; set; }
        public string email { get; set; }
        public string contrasena { get; set; }

        public void ActualizarDatosPersonales(string nombre, string correo, string apellido)
        {

        }

        public void CambiarContraseña(string nuevaClave)
        {

        }
    }
}
