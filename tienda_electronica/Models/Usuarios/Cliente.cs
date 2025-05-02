namespace tienda_electronica.Models.Usuarios
{
    public class Cliente : Persona
    {
        public int idCliente { get; set; }
        public int telefono { get; set; }
        public string direccion {  get; set; }

    }
}
