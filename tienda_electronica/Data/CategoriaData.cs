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
    }
}
