using MySql.Data.MySqlClient;
using Microsoft.Extensions.Configuration;
using System;

namespace tienda_electronica.Data
{
    public class Conexion
    {
        private readonly IConfiguration _config;
        private readonly string _connectionString;

        public Conexion(IConfiguration config)
        {
            _config = config;
            _connectionString = _config.GetConnectionString("MySqlConnection");
        }

        public MySqlConnection ObtenerConexion()
        {
            return new MySqlConnection(_connectionString);
        }
    }
}
