using MySql.Data.MySqlClient;

namespace Datos
{
    /// <sumary> Conexión con la base de datos </sumary>
    public static class Conexion
    {
        private const string Cadena = "Server=localhost;Database=ventasdb;Uid=root;Pwd=root;";
        public static MySqlConnection Obtener() => new MySqlConnection(Cadena);
    }
}
