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

        private string GuardarImagen(IFormFile imagen)
        {
            var nombreArchivo = Guid.NewGuid().ToString() + Path.GetExtension(imagen.FileName);
            var ruta = Path.Combine("wwwroot", "img", "productos", nombreArchivo);

            using (var stream = new FileStream(ruta, FileMode.Create))
            {
                imagen.CopyTo(stream);
            }

            return Path.Combine("/img/productos", nombreArchivo);
        }

        public int AgregarProducto(Producto producto, IFormFile imagenPrincipal, List<IFormFile> imagenesAdicionales)
        {
            int idProductoNuevo = 0;
            using (var connection = _conexion.ObtenerConexion())
            {
                connection.Open();
                using (var transaction = connection.BeginTransaction())
                {
                    try
                    {
                        var queryProducto = @"INSERT INTO productos 
                                      (id_categoria, nombre, descripcion, precio, stock, precio_descuento, activo)
                                      VALUES (@idCategoria, @nombre, @descripcion, @precio, @stock, @precioDescuento, @estado);
                                      SELECT LAST_INSERT_ID();";
                        using (var cmdProducto = new MySqlCommand(queryProducto, connection, transaction))
                        {
                            cmdProducto.Parameters.AddWithValue("@idCategoria", producto.idCategoria);
                            cmdProducto.Parameters.AddWithValue("@nombre", producto.nombre);
                            cmdProducto.Parameters.AddWithValue("@descripcion", producto.descripcion);
                            cmdProducto.Parameters.AddWithValue("@precio", producto.precio);
                            cmdProducto.Parameters.AddWithValue("@stock", producto.stock);
                            cmdProducto.Parameters.AddWithValue("@precioDescuento", producto.precioDescuento);
                            cmdProducto.Parameters.AddWithValue("@estado", producto.estado);

                            idProductoNuevo = Convert.ToInt32(cmdProducto.ExecuteScalar());
                        }

                        if (imagenPrincipal != null && imagenPrincipal.Length > 0)
                        {
                            var rutaImagenPrincipal = GuardarImagen(imagenPrincipal);

                            var queryImagenPrincipal = @"INSERT INTO imagenes_producto (id_producto, ruta_imagen)
                                                 VALUES (@idProducto, @rutaImagen);";

                            using (var cmdImagen = new MySqlCommand(queryImagenPrincipal, connection, transaction))
                            {
                                cmdImagen.Parameters.AddWithValue("@idProducto", idProductoNuevo);
                                cmdImagen.Parameters.AddWithValue("@rutaImagen", rutaImagenPrincipal);
                                cmdImagen.ExecuteNonQuery();
                            }
                        }

                        if (imagenesAdicionales != null && imagenesAdicionales.Count > 0)
                        {
                            foreach (var imagen in imagenesAdicionales.Take(4))
                            {
                                if (imagen != null && imagen.Length > 0)
                                {
                                    var rutaImagen = GuardarImagen(imagen);

                                    var queryImagenAdicional = @"INSERT INTO imagenes_producto (id_producto, ruta_imagen)
                                                         VALUES (@idProducto, @rutaImagen);";

                                    using (var cmdImagenAdicional = new MySqlCommand(queryImagenAdicional, connection, transaction))
                                    {
                                        cmdImagenAdicional.Parameters.AddWithValue("@idProducto", idProductoNuevo);
                                        cmdImagenAdicional.Parameters.AddWithValue("@rutaImagen", rutaImagen);
                                        cmdImagenAdicional.ExecuteNonQuery();
                                    }
                                }
                            }
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

            return idProductoNuevo;
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

        /*public Producto ObtenerProductoPorId(int idProducto)
        {
            Producto producto = null;

            using (var conn = _conexion.ObtenerConexion())
            {
                conn.Open();
                var query = @"SELECT p.*, 
                         (SELECT ruta_imagen 
                          FROM imagenes_producto 
                          WHERE id_producto = p.id_producto 
                          LIMIT 1) AS rutaImagen
                      FROM productos p
                      WHERE p.id_producto = @idProducto";

                using (var cmd = new MySqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@idProducto", idProducto);

                    using (var reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            producto = new Producto
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
                            };
                        }
                    }
                }
            }

            return producto;
        }*/

        public Producto ObtenerProductoPorId(int idProducto)
        {
            Producto producto = null;

            using (var conn = _conexion.ObtenerConexion())
            {
                conn.Open();

                var queryProducto = @"SELECT * FROM productos WHERE id_producto = @idProducto";

                using (var cmd = new MySqlCommand(queryProducto, conn))
                {
                    cmd.Parameters.AddWithValue("@idProducto", idProducto);

                    using (var reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            producto = new Producto
                            {
                                idProducto = Convert.ToInt32(reader["id_producto"]),
                                idCategoria = Convert.ToInt32(reader["id_categoria"]),
                                nombre = reader["nombre"].ToString(),
                                descripcion = reader["descripcion"].ToString(),
                                precio = Convert.ToDecimal(reader["precio"]),
                                stock = Convert.ToInt32(reader["stock"]),
                                precioDescuento = reader["precio_descuento"] != DBNull.Value ? Convert.ToDecimal(reader["precio_descuento"]) : 0,
                                estado = Convert.ToBoolean(reader["activo"]),
                                //rutaImagen = reader["rutaImagen"].ToString(),
                                ImagenesExtras = new List<string>()
                            };
                        }
                    }
                }

                if (producto != null)
                {
                    var queryImagenes = @"SELECT ruta_imagen FROM imagenes_producto WHERE id_producto = @idProducto";

                    using (var cmdImg = new MySqlCommand(queryImagenes, conn))
                    {
                        cmdImg.Parameters.AddWithValue("@idProducto", idProducto);

                        using (var readerImg = cmdImg.ExecuteReader())
                        {
                            while (readerImg.Read())
                            {
                                producto.ImagenesExtras.Add(readerImg["ruta_imagen"].ToString());
                            }
                        }
                    }

                    if (producto.ImagenesExtras.Any())
                    {
                        producto.rutaImagen = producto.ImagenesExtras[0];
                    }
                }
            }

            return producto;
        }


        public void EditarProducto(Producto producto, IFormFile imagenPrincipal, List<IFormFile> imagenesAdicionales)
        {
            using (var connection = _conexion.ObtenerConexion())
            {
                connection.Open();
                using (var transaction = connection.BeginTransaction())
                {
                    try
                    {
                        var queryUpdateProducto = @"UPDATE productos SET
                                                id_categoria = @idCategoria,
                                                nombre = @nombre,
                                                descripcion = @descripcion,
                                                precio = @precio,
                                                stock = @stock,
                                                precio_descuento = @precioDescuento,
                                                activo = @estado
                                            WHERE id_producto = @idProducto";

                        using (var cmdProducto = new MySqlCommand(queryUpdateProducto, connection, transaction))
                        {
                            cmdProducto.Parameters.AddWithValue("@idCategoria", producto.idCategoria);
                            cmdProducto.Parameters.AddWithValue("@nombre", producto.nombre);
                            cmdProducto.Parameters.AddWithValue("@descripcion", producto.descripcion);
                            cmdProducto.Parameters.AddWithValue("@precio", producto.precio);
                            cmdProducto.Parameters.AddWithValue("@stock", producto.stock);
                            cmdProducto.Parameters.AddWithValue("@precioDescuento", producto.precioDescuento);
                            cmdProducto.Parameters.AddWithValue("@estado", producto.estado);
                            cmdProducto.Parameters.AddWithValue("@idProducto", producto.idProducto);

                            cmdProducto.ExecuteNonQuery();
                        }

                        if (imagenPrincipal != null && imagenPrincipal.Length > 0)
                        {
                            var queryDeleteImagenes = "DELETE FROM imagenes_producto WHERE id_producto = @idProducto";
                            using (var cmdDeleteImagenes = new MySqlCommand(queryDeleteImagenes, connection, transaction))
                            {
                                cmdDeleteImagenes.Parameters.AddWithValue("@idProducto", producto.idProducto);
                                cmdDeleteImagenes.ExecuteNonQuery();
                            }

                            var rutaImagenPrincipal = GuardarImagen(imagenPrincipal);

                            var queryInsertImagenPrincipal = @"INSERT INTO imagenes_producto (id_producto, ruta_imagen)
                                                        VALUES (@idProducto, @rutaImagen)";
                            using (var cmdInsertImagen = new MySqlCommand(queryInsertImagenPrincipal, connection, transaction))
                            {
                                cmdInsertImagen.Parameters.AddWithValue("@idProducto", producto.idProducto);
                                cmdInsertImagen.Parameters.AddWithValue("@rutaImagen", rutaImagenPrincipal);
                                cmdInsertImagen.ExecuteNonQuery();
                            }

                            if (imagenesAdicionales != null && imagenesAdicionales.Count > 0)
                            {
                                foreach (var imagen in imagenesAdicionales.Take(4))
                                {
                                    if (imagen != null && imagen.Length > 0)
                                    {
                                        var rutaImagen = GuardarImagen(imagen);

                                        var queryInsertImagenAdicional = @"INSERT INTO imagenes_producto (id_producto, ruta_imagen)
                                                                    VALUES (@idProducto, @rutaImagen)";
                                        using (var cmdInsertImagenAdicional = new MySqlCommand(queryInsertImagenAdicional, connection, transaction))
                                        {
                                            cmdInsertImagenAdicional.Parameters.AddWithValue("@idProducto", producto.idProducto);
                                            cmdInsertImagenAdicional.Parameters.AddWithValue("@rutaImagen", rutaImagen);
                                            cmdInsertImagenAdicional.ExecuteNonQuery();
                                        }
                                    }
                                }
                            }
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

        public List<Producto> ObtenerPorCategoria(int idCategoria)
        {
            List<Producto> productos = new List<Producto>();

            using (var connection = _conexion.ObtenerConexion())
            {
                connection.Open();

                var queryObtenerPorCategoria = @"SELECT p.*, 
                                 (SELECT ruta_imagen 
                                  FROM imagenes_producto 
                                  WHERE id_producto = p.id_producto 
                                  LIMIT 1) AS rutaImagen
                              FROM productos p 
                              WHERE id_categoria = @idCategoria";

                using (var cmd = new MySqlCommand(queryObtenerPorCategoria, connection))
                {
                    cmd.Parameters.AddWithValue("@idCategoria", idCategoria);

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
                                rutaImagen = reader["rutaImagen"] == DBNull.Value ? "" : reader["rutaImagen"].ToString(),

                            });
                        }
                    }
                }
            }

            return productos;
        }
    }
}
