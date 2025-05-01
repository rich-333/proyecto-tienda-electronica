namespace tienda_electronica.Models.Ventas
{
    public class Venta
    {
        public int idPedido { get; set; }
        public int idCliente { get; set; }
        public string nombreCliente { get; set; }
        public string apellidoCliente { get; set; }
        public DateTime fechaPedido { get; set; }

        public decimal total { get; set; }
        public string estado { get; set; }

    }
}
