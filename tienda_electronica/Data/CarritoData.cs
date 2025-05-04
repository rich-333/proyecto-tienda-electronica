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
                        while (reader.Read())
                        {
                            lista.Add(new Carrito
                            {
                                idCarrito = reader.GetInt32("id_carrito"),
                                idProducto = reader.GetInt32("id_producto"),
                                cantidad = reader.GetInt32("cantidad"),
                                fechaAgregado = reader.GetDateTime("fecha_agregado"),
                                nombreProducto = reader.GetString("NombreProducto"),
                                precio = reader.GetDecimal("precio"),
                                precioDescuento = reader.GetDecimal("precio_descuento"),
                                imagenUrl = reader.GetString("ruta_imagen"),
                            });
                        }
                    }
                }
            }
            return lista;
        }

        public void AgregarProductoAlCarrito(int idCliente, int idProducto, int cantidad)
        {
            using (var connection = _conexion.ObtenerConexion())
            {
                connection.Open();

                var checkQuery = @"SELECT COUNT(*) FROM carrito WHERE id_cliente = @idCliente AND id_producto = @idProducto";

                using (var checkCmd = new MySqlCommand(checkQuery, connection))
                {
                    checkCmd.Parameters.AddWithValue("@idCliente", idCliente);
                    checkCmd.Parameters.AddWithValue("@idProducto", idProducto);
                    var existe = Convert.ToInt32(checkCmd.ExecuteScalar()) > 0;

                    if (existe)
                    {
                        var updateQuery = @"UPDATE carrito 
                                    SET cantidad = cantidad + @cantidad, fecha_agregado = NOW()
                                    WHERE id_cliente = @idCliente AND id_producto = @idProducto";

                        using (var updateCmd = new MySqlCommand(updateQuery, connection))
                        {
                            updateCmd.Parameters.AddWithValue("@idCliente", idCliente);
                            updateCmd.Parameters.AddWithValue("@idProducto", idProducto);
                            updateCmd.Parameters.AddWithValue("@cantidad", cantidad);
                            updateCmd.ExecuteNonQuery();
                        }
                    }
                    else
                    {
                        var insertQuery = @"INSERT INTO carrito (id_cliente, id_producto, cantidad, fecha_agregado)
                                    VALUES (@idCliente, @idProducto, @cantidad, NOW())";

                        using (var insertCmd = new MySqlCommand(insertQuery, connection))
                        {
                            insertCmd.Parameters.AddWithValue("@idCliente", idCliente);
                            insertCmd.Parameters.AddWithValue("@idProducto", idProducto);
                            insertCmd.Parameters.AddWithValue("@cantidad", cantidad);
                            insertCmd.ExecuteNonQuery();
                        }
                    }
                }
            }
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
