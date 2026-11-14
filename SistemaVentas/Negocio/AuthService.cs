using System;
using System.Security.Cryptography;
using System.Text;
using Datos;

namespace Negocio
{
    // Autenticación de usuarios.
    public class AuthService
    {
        // Convierte una clave a hash SHA-256 en hexadecimal minúscula (64 caracteres).
        public static string Hash(string clave)
        {
            var bytes = SHA256.HashData(Encoding.UTF8.GetBytes(clave));
            return Convert.ToHexString(bytes).ToLowerInvariant();
        }

        // Valida el usuario comparando el hash de la clave.
        public bool Login(string u, string c) => RepositorioFactory.Usuarios().Validar(u, Hash(c));
    }
}
