using MySql.Data.MySqlClient;
using System.Data;
using tienda_electronica.Models.Favoritos;

namespace tienda_electronica.Data
{
    public class FavoritoData
    {
        private readonly Conexion _conexion;

        public FavoritoData (IConfiguration config)
        {
            _conexion = new Conexion(config);
        }

        public List<Favorito> ObtenerFavoritosPorCliente(int idCliente)
        {
            var lista = new List<Favorito>();

            using (var connection = _conexion.ObtenerConexion())
            {
                connection.Open();

                using (var cmd = new MySqlCommand("ObtenerFavoritos", connection))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@idCliente", idCliente);

                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            lista.Add(new Favorito 
                            { 
                                idFavorito = reader.GetInt32("id_favorito"),
                                idProducto = reader.GetInt32("id_producto"),
                                fecha_agregado = reader.GetDateTime("fecha_agregado"),
                                nombreProducto = reader.GetString("nombre"),
                                precio = reader.GetDecimal("precio"),
                                precioDescuento = reader["precio_descuento"] == DBNull.Value ? 0 : Convert.ToDecimal(reader["precio_descuento"]),
                                imagenUrl = reader.GetString("ruta_imagen"),
                            });
                        }
                    }
                }
            }
            return lista;
        }

        public void AgregarFavorito(int idCliente, int idProducto)
        {
            using (var connection = _conexion.ObtenerConexion())
            {
                connection.Open();

                var query = @"INSERT INTO favoritos (id_cliente, id_producto, fecha_agregado) VALUES (@idCliente, @idProducto, NOW())";

                using (var cmd = new MySqlCommand (query, connection))
                {
                    cmd.Parameters.AddWithValue("@idCliente", idCliente);
                    cmd.Parameters.AddWithValue("@idProducto", idProducto);

                    cmd.ExecuteNonQuery();
                }
            }
        }

        public void EliminarFavorito(int idFavorito)
        {
            using (var connection = _conexion.ObtenerConexion())
            {
                connection.Open();

                var query = @"DELETE FROM favoritos WHERE id_favorito = @idFavorito";

                using (var cmd = new MySqlCommand(query, connection))
                {
                    cmd.Parameters.AddWithValue("@idFavorito", idFavorito);
                    cmd.ExecuteNonQuery();
                }
            }
        }

        public bool EstaEnFavoritos(int idCliente, int idProducto)
        {
            using (var connection = _conexion.ObtenerConexion())
            {
                connection.Open();

                var query = @"SELECT COUNT(*) FROM favoritos WHERE id_cliente = @idCliente AND id_producto = @idProducto";

                using (var cmd = new MySqlCommand(query, connection))
                {
                    cmd.Parameters.AddWithValue("@idCliente", idCliente);
                    cmd.Parameters.AddWithValue("@idProducto", idProducto);

                    var count = Convert.ToInt32(cmd.ExecuteScalar());
                    return count > 0;
                }
            }
        }
    }
}
