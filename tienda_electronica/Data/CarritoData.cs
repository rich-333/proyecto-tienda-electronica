using MySql.Data.MySqlClient;
using System.Data;
using tienda_electronica.Models.Carrito;

namespace tienda_electronica.Data
{
    public class CarritoData
    {
        private readonly Conexion _conexion;

        public CarritoData (IConfiguration config)
        {
            _conexion = new Conexion(config);
        }

        public List<Carrito> ObtenerCarritoPorCliente(int idCliente)
        {
            var lista = new List<Carrito>();

            using (var connection = _conexion.ObtenerConexion())
            {
                connection.Open();

                using (var cmd = new MySqlCommand("ObtenerCarrito", connection))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@idCliente", idCliente);

                    using (var reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            lista.Add(new Carrito
                            {
                                idCarrito = reader.GetInt32("id_carrito"),
                                idProducto = reader.GetInt32("id_producto"),
                                cantidad = reader.GetInt32("cantidad"),
                                fechaAgregado = reader.GetDateTime("fecha_agregado"),
                                nombreProducto = reader.GetString("NombreProducto"),
                                precio = reader.GetDecimal("precio"),
                                imagenUrl = reader.GetString("ruta_imagen"),
                            });
                        }
                    }
                }
            }
            return lista;
        }

        public void EliminarProductoDelCarrito (int idCarrito)
        {
            using (var connection = _conexion.ObtenerConexion())
            {
                connection.Open();

                var query = @"DELETE FROM carrito WHERE id_carrito = @idCarrito";

                using (var cmd = new MySqlCommand(query, connection))
                {
                    cmd.Parameters.AddWithValue ("@idCarrito", idCarrito);
                    cmd.ExecuteNonQuery();
                }
            }
        }
    }
}
