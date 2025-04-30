using MySql.Data.MySqlClient;
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
    }
}
