using tienda_electronica.Models.Productos;

namespace tienda_electronica.Models.Favoritos
{
    public class Favorito
    {
        public int idFavorito { get; set; }
        public int idCliente { get; set; }
        public int idProducto { get; set; }
        public DateTime fecha_agregado { get; set; }

        public void AgregarFavorito(Producto producto)
        {

        }

        public void EliminarFavorito(int idProducto)
        {

        }

        public void ListarFavoritos(int idUsuario)
        {

        }
    }
}
