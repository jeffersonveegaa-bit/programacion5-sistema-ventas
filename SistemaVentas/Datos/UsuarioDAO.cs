using System;
using MySql.Data.MySqlClient;

namespace Datos
{
    /// <summary> Valida credenciales de usuario </summary>
    public class UsuarioDAO
    {
        public bool Validar(string usuario, string clave)
        {
            using var conexion = Conexion.Obtener(); conexion.Open();
            using var comando = new MySqlCommand("SELECT COUNT(*) FROM usuarios WHERE usuario=@usuario AND clave=@clave", conexion);
            comando.Parameters.AddWithValue("@usuario", usuario); comando.Parameters.AddWithValue("@clave", clave);
            return Convert.ToInt32(comando.ExecuteScalar()) > 0;
        }
    }
}
