namespace tienda_electronica.Models.Usuarios
{
    public class Persona
    {
        public string nombre { get; set; }
        public string apellido { get; set; }
        public string email { get; set; }
        private string contrasena { get; set; }
    }
}
