using MySql.Data.MySqlClient;
using tienda_electronica.Models.Usuarios;

namespace tienda_electronica.Data
{
    public class ClienteData
    {
        private readonly Conexion _conexion;

        public ClienteData (IConfiguration config)
        {
            _conexion = new Conexion(config);
        }

        public List<Cliente> ObtenerClientes()
        {
            var clientes = new List<Cliente>();

            using (var connection = _conexion.ObtenerConexion())
            {
                connection.Open();

                var query = @"SELECT * FROM clientes";

                using (var cmd = new MySqlCommand(query, connection))
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read()) 
                    {
                        clientes.Add(new Cliente
                        {
                            idCliente = Convert.ToInt32(reader["id_cliente"]),
                            nombre = reader["nombre"].ToString(),
                            apellido = reader["apellido"].ToString(),
                            email = reader["email"].ToString(),
                            contrasena = reader["contrasena"].ToString(),
                            telefono = Convert.ToInt32(reader["telefono"]),
                            direccion = reader["direccion"].ToString()
                        });
                    }
                }
            }
            return clientes;
        }

        public Cliente ObtenerClientePorId (int idCliente)
        {
            Cliente cliente = null;

            using (var connection = _conexion.ObtenerConexion())
            {
                connection.Open();

                var queryObtenerId = @"SELECT * FROM clientes WHERE id_cliente = @idCliente";

                using (var cmd = new MySqlCommand(queryObtenerId, connection))
                {
                    cmd.Parameters.AddWithValue("@idCliente", idCliente);

                    using (var reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            cliente = new Cliente 
                            {
                                idCliente = Convert.ToInt32(reader["id_cliente"]),
                                email = reader["email"].ToString(),
                                contrasena = reader["contrasena"].ToString(),
                                telefono = Convert.ToInt32(reader["telefono"]),
                                direccion = reader["direccion"].ToString()
                            };
                        }
                    }
                }
            }

            return cliente;
        }

        public void EditarCliente(Cliente cliente)
        {
            using (var connection = _conexion.ObtenerConexion())
            {
                connection.Open();

                using (var transaction = connection.BeginTransaction())
                {
                    try
                    {
                        var queryEdit = @"UPDATE clientes SET
                                        email = @email,
                                        contrasena = @contrasena,
                                        telefono = @telefono,
                                        direccion = @direccion
                                        WHERE id_cliente = @idCliente";

                        using (var cmd = new MySqlCommand(queryEdit, connection, transaction))
                        {
                            cmd.Parameters.AddWithValue("@email", cliente.email);
                            cmd.Parameters.AddWithValue("@contrasena", cliente.contrasena);
                            cmd.Parameters.AddWithValue("@telefono", cliente.telefono);
                            cmd.Parameters.AddWithValue("@direccion", cliente.direccion);
                            cmd.Parameters.AddWithValue("@idCliente", cliente.idCliente);

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
