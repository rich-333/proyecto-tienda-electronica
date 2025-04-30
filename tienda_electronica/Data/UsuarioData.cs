using MySql.Data.MySqlClient;
using Mysqlx.Crud;
using tienda_electronica.Models.Usuarios;

namespace tienda_electronica.Data
{
    public class UsuarioData
    {
        private readonly Conexion _conexion;

        public UsuarioData (IConfiguration config)
        {
            _conexion = new Conexion (config);  
        }

        public List<Usuario> ObtenerUsuarios()
        {
            var usuarios = new List<Usuario>();

            using (var connection = _conexion.ObtenerConexion())
            {
                connection.Open();
                var query = @"SELECT * FROM usuarios";

                using (var cmd = new MySqlCommand(query, connection))
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        usuarios.Add(new Usuario
                        {
                            nombre = reader["nombre"].ToString(),
                            apellido = reader["apellido"].ToString(),
                            email = reader["email"].ToString(),
                            contrasena = reader["contrasena"].ToString(),
                            rutaFotoEmpleado = reader["ruta_foto_empleado"].ToString(),
                            rol = reader["rol"].ToString()
                        });
                    }
                }
            }

            return usuarios;
        }

        public string GuardarImagen(IFormFile imagen)
        {
            var nombreArchivo = Guid.NewGuid().ToString() + Path.GetExtension(imagen.FileName);
            var ruta = Path.Combine("wwwroot", "img", "usuarios", nombreArchivo);

            using (var stream = new FileStream(ruta, FileMode.Create))
            {
                imagen.CopyTo(stream);
            }

            return Path.Combine("/img/usuarios", nombreArchivo);
        }

        public int AgregarUsuario(Usuario usuario, IFormFile imagenUsuario) 
        {
            int idUsuarioNuevo = 0;

            string rutaImagen = GuardarImagen(imagenUsuario);
            usuario.rutaFotoEmpleado = rutaImagen;

            using (var connection = _conexion.ObtenerConexion())
            {
                connection.Open();

                using (var transaction = connection.BeginTransaction())
                {
                    try
                    {
                        var queryUsuario = @"INSERT INTO usuarios
                                        (nombre, apellido, email, contrasena, rol, ruta_foto_empleado)
                                        VALUES (@nombre, @apellido, @email, @contrasena, @rol, @fotoEmpleado);
                                        SELECT LAST_INSERT_ID();";
                        using (var cmdUsuario = new MySqlCommand(queryUsuario, connection, transaction))
                        {
                            cmdUsuario.Parameters.AddWithValue("@nombre", usuario.nombre);
                            cmdUsuario.Parameters.AddWithValue("@apellido", usuario.apellido);
                            cmdUsuario.Parameters.AddWithValue("@email", usuario.email);
                            cmdUsuario.Parameters.AddWithValue("@contrasena", usuario.contrasena);
                            cmdUsuario.Parameters.AddWithValue("@rol", usuario.rol);
                            cmdUsuario.Parameters.AddWithValue("@fotoEmpleado", usuario.rutaFotoEmpleado);

                            idUsuarioNuevo = Convert.ToInt32(cmdUsuario.ExecuteScalar());
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
            return idUsuarioNuevo;
        }

        public void EliminarUsuarios (int idUsuario)
        {
            using (var connection = _conexion.ObtenerConexion())
            {
                connection.Open();

                string rutaImagen = null;
                var querySelect = @"SELECT ruta_foto_empleado FROM usuarios WHERE id_usuario = @idUsuario";
                using (var cmdSelect = new MySqlCommand(querySelect, connection))
                {
                    cmdSelect.Parameters.AddWithValue("@idUsuario", idUsuario);
                    var resultado = cmdSelect.ExecuteScalar();
                    if (resultado != null)
                        rutaImagen = resultado.ToString();
                }

                var queryDelete = @"DELETE FROM usuarios WHERE id_usuario = @idUsuario";
                using (var cmd = new MySqlCommand(queryDelete, connection)) 
                { 
                    cmd.Parameters.AddWithValue("@idUsuario", idUsuario);
                    cmd.ExecuteNonQuery(); 
                }

                if (!string.IsNullOrEmpty(rutaImagen))
                {
                    var rutaFisica = Path.Combine("wwwroot", rutaImagen.TrimStart('/').Replace('/', Path.DirectorySeparatorChar));
                    if (File.Exists(rutaFisica))
                        File.Delete(rutaFisica);
                }
            }
        }
    }
}
