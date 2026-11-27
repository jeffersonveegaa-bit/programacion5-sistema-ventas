using MySql.Data.MySqlClient;

namespace Datos
{
    /// <sumary> Conexión con la base de datos </sumary>
    public static class Conexion
    {
        private const string Cadena = "Server=localhost;Database=ventasdb;Uid=root;Pwd=root;";

        /// <sumary> Método estático para llamar a la conexion </sumary>
        public static MySqlConnection Obtener() => new MySqlConnection(Cadena);
    }
}
