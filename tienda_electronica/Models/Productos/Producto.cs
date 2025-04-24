namespace tienda_electronica.Models.Productos
{
    public class Producto
    {
        public int idProducto { get; set; }
        public int idCategoria { get; set; }
        public string nombre { get; set; } 
        public string descripcion {  get; set; }
        public decimal precio { get; set; }
        public int stock { get; set; }
        public decimal precioDescuento { get; set; }

        public bool estado { get; set; }
    }
}
