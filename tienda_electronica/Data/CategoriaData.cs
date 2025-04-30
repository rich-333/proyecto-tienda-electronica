using MySql.Data.MySqlClient;
using System.Collections.Generic;
using tienda_electronica.Models.Categorias;

namespace tienda_electronica.Data
{
    public class CategoriaData
    {
        public readonly Conexion _conexion;

        public CategoriaData(IConfiguration config)
        {
            _conexion = new Conexion(config);
        }

        public List<Categoria> ObtenerCategorias()
        {
            List<Categoria> categorias = new List<Categoria>();

            using (var connection = _conexion.ObtenerConexion())
            {
                connection.Open();
                var query = "SELECT * FROM categorias";

                using (var cmd = new MySqlCommand(query, connection))
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read()) 
                    {
                        categorias.Add(new Categoria
                        {
                            idCategoria = Convert.ToInt32(reader["id_categoria"]),
                            nombre = reader["nombre"].ToString(),
                            descripcion = reader["descripcion"].ToString()
                        });
                    }
                }
            }

            return categorias;
        }

        public void EliminarCategoria(int idCategoria)
        {
            using (var connection = _conexion.ObtenerConexion())
            {
                connection.Open();
                var query = "DELETE FROM categorias WHERE id_categoria = @id";
                using (var cmd = new MySqlCommand(query, connection))
                {
                    cmd.Parameters.AddWithValue("@id", idCategoria);
                    cmd.ExecuteNonQuery();
                }
            }
        }

        public int AgregarCategoria(Categoria categoria)
        {
            int idCategoriaNueva = 0;

            using (var connection = _conexion.ObtenerConexion())
            {
                connection.Open();

                using (var transaction = connection.BeginTransaction())
                {
                    try
                    {
                        var queryCategoria = @"INSERT INTO categorias 
                            (id_categoria, nombre, descripcion)
                            VALUES (@idCategoria, @nombre, @descripcion);
                            SELECT LAST_INSERT_ID();";
                        using (var cmdCategoria = new MySqlCommand(queryCategoria, connection, transaction))
                        {
                            cmdCategoria.Parameters.AddWithValue("@idCategoria", categoria.idCategoria);
                            cmdCategoria.Parameters.AddWithValue("@nombre", categoria.nombre);
                            cmdCategoria.Parameters.AddWithValue("@descripcion", categoria.descripcion);

                            idCategoriaNueva = Convert.ToInt32(cmdCategoria.ExecuteScalar());
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

            return idCategoriaNueva;
        }

        public Categoria ObtenerCategoriaPorId(int idCategoria)
        {
            Categoria categoria = null;

            using (var connection = _conexion.ObtenerConexion())
            {
                connection.Open();
                var queryObtenerCategoriaPorId = @"SELECT * FROM categorias
                                                    WHERE id_categoria = @idCategoria";
                using (var cmd = new MySqlCommand(queryObtenerCategoriaPorId, connection))
                {
                    cmd.Parameters.AddWithValue("@idCategoria", idCategoria);
                    using (var reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            categoria = new Categoria
                            {
                                idCategoria = Convert.ToInt32(reader["id_categoria"]),
                                nombre = reader["nombre"].ToString(),
                                descripcion = reader["descripcion"].ToString()
                            };
                        }
                    }
                }
            }

            return categoria;
        }

        public void EditarCategoria(Categoria categoria)
        {
            using (var connection = _conexion.ObtenerConexion())
            {
                connection.Open();

                using ( var transaction = connection.BeginTransaction())
                {
                    try
                    {
                        var queryEditCategoria = @"UPDATE categorias SET 
                                                 nombre = @nombre,
                                                 descripcion = @descripcion
                                                 WHERE id_categoria = @idCategoria";
                        using (var cmdCategoria = new MySqlCommand(queryEditCategoria, connection, transaction))
                        {
                            cmdCategoria.Parameters.AddWithValue("@nombre", categoria.nombre);
                            cmdCategoria.Parameters.AddWithValue("@descripcion", categoria.descripcion);
                            cmdCategoria.Parameters.AddWithValue("@idCategoria", categoria.idCategoria);

                            cmdCategoria.ExecuteNonQuery();
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
