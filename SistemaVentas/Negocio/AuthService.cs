using System;
using System.Security.Cryptography;
using System.Text;
using Datos;

namespace Negocio
{
    /// <summary> Autenticación de usuarios. </summary>
    public class AuthService
    {
        /// <summary> Convierte una clave a hash SHA-256 en hexadecimal minúscula (64 caracteres). </summary>
        public static string Hash(string clave)
        {
            var bytes = SHA256.HashData(Encoding.UTF8.GetBytes(clave));
            return Convert.ToHexString(bytes).ToLowerInvariant();
        }

        /// <summary> Valida el usuario comparando el hash de la clave. </summary>
        public bool Login(string u, string c) => RepositorioFactory.Usuarios().Validar(u, Hash(c));
    }
}
