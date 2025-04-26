using MySql.Data.MySqlClient;
using System.Collections.Generic;
using tienda_electronica.Models.Productos;

namespace tienda_electronica.Data
{
    public class ProductoData
    {
        private readonly Conexion _conexion;

        public ProductoData (IConfiguration config)
        {
            _conexion = new Conexion(config);
        }

        public List<Producto> ObtenerProductos()
        {
            var productos = new List<Producto>();

            using (var conn = _conexion.ObtenerConexion())
            {
                conn.Open();
                var query = @"SELECT p.*, 
                                 (SELECT ruta_imagen 
                                  FROM imagenes_producto 
                                  WHERE id_producto = p.id_producto 
                                  LIMIT 1) AS rutaImagen
                              FROM productos p";
                using (var cmd = new MySqlCommand(query, conn))
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        productos.Add(new Producto
                        {
                            idProducto = reader["id_producto"] == DBNull.Value ? 0 : Convert.ToInt32(reader["id_producto"]),
                            idCategoria = reader["id_categoria"] == DBNull.Value ? 0 : Convert.ToInt32(reader["id_categoria"]),
                            nombre = reader["nombre"] == DBNull.Value ? "" : reader["nombre"].ToString(),
                            descripcion = reader["descripcion"] == DBNull.Value ? "" : reader["descripcion"].ToString(),
                            precio = reader["precio"] == DBNull.Value ? 0 : Convert.ToDecimal(reader["precio"]),
                            stock = reader["stock"] == DBNull.Value ? 0 : Convert.ToInt32(reader["stock"]),
                            precioDescuento = reader["precio_descuento"] == DBNull.Value ? 0 : Convert.ToDecimal(reader["precio_descuento"]),
                            estado = reader["activo"] == DBNull.Value ? false : Convert.ToBoolean(reader["activo"]),
                            rutaImagen = reader["rutaImagen"].ToString()
                        });
                    }
                }
            }

            return productos;
        }

        public void AgregarProducto(Producto producto)
        {
            using (var connection = _conexion.ObtenerConexion())
            {
                connection.Open();
                
            }
        }

        public bool ProductoTienePedidos(int idProducto)
        {
            using (var connection = _conexion.ObtenerConexion())
            {
                connection.Open();
                var query = "SELECT COUNT(*) FROM detalle_pedido WHERE id_producto = @id";
                using (var cmd = new MySqlCommand(query, connection))
                {
                    cmd.Parameters.AddWithValue("@id", idProducto);
                    var count = Convert.ToInt32(cmd.ExecuteScalar());
                    return count > 0;
                }
            }
        }

        public void EliminarProducto(int idProducto)
        {
            using (var connection = _conexion.ObtenerConexion())
            {
                connection.Open();
                var query = "DELETE FROM productos WHERE id_producto = @id";
                using (var cmd = new MySqlCommand(query, connection))
                {
                    cmd.Parameters.AddWithValue("@id", idProducto);
                    cmd.ExecuteNonQuery();
                }
            }
        }
    }
}
