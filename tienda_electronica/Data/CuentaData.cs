using MySql.Data.MySqlClient;
using tienda_electronica.Models.Usuarios;

namespace tienda_electronica.Data
{
    public class CuentaData
    {
        private readonly Conexion _conexion;

        public CuentaData (IConfiguration config)
        {
            _conexion = new Conexion(config);
        }

        public Usuario? AutenticarUsuario(string email, string contrasena)
        {
            using (var connection = _conexion.ObtenerConexion())
            {
                connection.Open();

                var query = "SELECT * FROM usuarios WHERE email = @email AND contrasena = @contrasena";

                using (var cmd = new MySqlCommand(query, connection))
                {
                    cmd.Parameters.AddWithValue("@email", email);
                    cmd.Parameters.AddWithValue("@contrasena", contrasena);

                    using (var reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            return new Usuario
                            {
                                idUsuario = Convert.ToInt32(reader["id_usuario"]),
                                nombre = reader["nombre"].ToString(),
                                apellido = reader["apellido"].ToString(),
                                email = reader["email"].ToString(),
                                contrasena = reader["contrasena"].ToString(),
                                rutaFotoEmpleado = reader["ruta_foto_empleado"].ToString(),
                                rol = reader["rol"].ToString()
                            };
                        }
                    }
                }
            }
            return null;
        }

        public Cliente ? AutenticarCliente (string email, string contrasena)
        {
            using (var connection = _conexion.ObtenerConexion())
            {
                connection.Open();

                var query = "SELECT * FROM clientes WHERE email = @email AND contrasena = @contrasena";
                using (var cmd = new MySqlCommand(query, connection))
                {
                    cmd.Parameters.AddWithValue("@email", email);
                    cmd.Parameters.AddWithValue("@contrasena", contrasena);

                    using (var reader = cmd.ExecuteReader())
                    {
                        if (reader.Read()) 
                        {
                            return new Cliente
                            {
                                idCliente = Convert.ToInt32(reader["id_cliente"]),
                                nombre = reader["nombre"].ToString(),
                                apellido = reader["apellido"].ToString(),
                                email = reader["email"].ToString(),
                                contrasena = reader["contrasena"].ToString(),
                                telefono = Convert.ToInt32(reader["telefono"]),
                                direccion = reader["direccion"].ToString()
                            };
                        }
                    }
                }
            }
            return null;
        }

        public void RegistrarCliente(Cliente cliente)
        {
            using (var connection = _conexion.ObtenerConexion())
            {
                connection.Open();
                var query = "INSERT INTO clientes (nombre, apellido, email, telefono, direccion, contrasena) VALUES (@nombre, @apellido, @email, @telefono, @direccion, @contrasena)";
                using (var cmd = new MySqlCommand(query, connection))
                {
                    cmd.Parameters.AddWithValue("@nombre", cliente.nombre);
                    cmd.Parameters.AddWithValue("@apellido", cliente.apellido);
                    cmd.Parameters.AddWithValue("@email", cliente.email);
                    cmd.Parameters.AddWithValue("@telefono", cliente.telefono);
                    cmd.Parameters.AddWithValue("@direccion", cliente.direccion);
                    cmd.Parameters.AddWithValue("@contrasena", cliente.contrasena);
                    cmd.ExecuteNonQuery();
                }
            }
        }
    }
}
