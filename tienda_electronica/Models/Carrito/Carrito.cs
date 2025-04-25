using tienda_electronica.Models.Productos;

namespace tienda_electronica.Models.Carrito
{
    public class Carrito
    {
        public int idCarrito { get; set; }
        public int idCliente { get; set; }
        public int idProducto { get; set; }
        public int cantidad { get; set; }
        public DateTime fechaAgregado { get; set; }

        public void AgregarProducto(Producto producto, int cantidad)
        {

        }

        public void EliminarProducto(int idProducto)
        {

        }

        public void VaciarCarrito()
        {

        }
    }
}
