using MySql.Data.MySqlClient;
using tienda_electronica.Models.Ventas;

namespace tienda_electronica.Data
{
    public class VentaData
    {
        private readonly Conexion _conexion;

        public VentaData (IConfiguration config)
        {
            _conexion = new Conexion(config);
        }

        public List<Venta> ObtenerVentas()
        {
            var ventas = new List<Venta>();

            using (var connection = _conexion.ObtenerConexion())
            {
                connection.Open();

                var query = @"SELECT  p.id_pedido, c.id_cliente, c.nombre, c.apellido, p.fecha_pedido, p.total, p.estado 
                            FROM pedidos p
                            JOIN clientes c ON p.id_cliente = c.id_cliente;";

                using (var cmd = new MySqlCommand(query, connection))
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read()) 
                    {
                        ventas.Add(new Venta
                        {
                            idPedido = Convert.ToInt32(reader["id_pedido"]),
                            idCliente = Convert.ToInt32(reader["id_cliente"]),
                            nombreCliente = reader["nombre"].ToString(),
                            apellidoCliente = reader["apellido"].ToString(),
                            fechaPedido = Convert.ToDateTime(reader["fecha_pedido"]),
                            total = Convert.ToDecimal(reader["total"]),
                            estado = reader["estado"].ToString()
                        });
                    }
                }
            }
            return ventas;
        }

        public Venta ObtenerVentaPorId(int idVenta)
        {
            Venta venta = null;

            using (var connection = _conexion.ObtenerConexion())
            {
                connection.Open();

                var queryObtenerId = @"SELECT * FROM pedidos WHERE id_pedido = @idPedido";

                using ( var cmd = new MySqlCommand( queryObtenerId, connection))
                {
                    cmd.Parameters.AddWithValue("@idPedido", idVenta);

                    using (var reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            venta = new Venta
                            {
                                idPedido = Convert.ToInt32(reader["id_pedido"]),
                                estado = reader["estado"].ToString()
                            };
                        }
                    }
                }

            }

            return venta;
        }

        public void EditarVenta(Venta venta)
        {
            using (var connection = _conexion.ObtenerConexion())
            {
                connection.Open();

                using (var transaction = connection.BeginTransaction())
                {
                    try
                    {
                        var queryEdit = @"UPDATE pedidos SET 
                                        estado = @estado
                                        WHERE id_pedido = @idPedido";
                        using (var cmd =  new MySqlCommand( queryEdit, connection, transaction))
                        {
                            cmd.Parameters.AddWithValue("@estado", venta.estado);
                            cmd.Parameters.AddWithValue("@idPedido", venta.idPedido);

                            cmd.ExecuteNonQuery();
                        }
                        transaction.Commit();
                    }
                    catch
                    {
                        transaction.Rollback();
                        throw;
                    }
                }
            }
        }
    }
}
