namespace tienda_electronica.Models.Productos
{
    public class Producto
    {
        public int idProducto { get; set; }
        public string nombre { get; set; } 
        public string descripcion {  get; set; }
        public double precio { get; set; }
        public int stock { get; set; }
        public double precioDescuento { get; set; }

        public string estado { get; set; }
    }
}
