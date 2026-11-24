using Entidades;
using MySql.Data.MySqlClient;
using System.Collections.Generic;

namespace Datos
{
    /// <summary> DAO de clientes </summary>
    public class ClienteDAO : IRepositorio<Cliente, int>
    {
        public List<Cliente> ObtenerTodos()
        {
            var lista = new List<Cliente>();
            using var conexion = Conexion.Obtener(); conexion.Open();
            using var comando = new MySqlCommand("SELECT id,nombre,telefono,correo FROM clientes", conexion);
            using var lector = comando.ExecuteReader();
            while (lector.Read())
                lista.Add(new Cliente
                {
                    Id = lector.GetInt32(0),
                    Nombre = lector.GetString(1),
                    Telefono = lector.IsDBNull(2) ? "" : lector.GetString(2),
                    Correo = lector.IsDBNull(3) ? "" : lector.GetString(3)
                });
            return lista;
        }
        public void Insertar(Cliente cliente) => Ejecutar("INSERT INTO clientes (nombre,telefono,correo) VALUES (@nombre,@telefono,@correo)", cliente);
        public void Actualizar(Cliente cliente) => Ejecutar("UPDATE clientes SET nombre=@nombre,telefono=@telefono,correo=@correo WHERE id=@id", cliente);
        public void Eliminar(int id)
        {
            using var conexion = Conexion.Obtener(); conexion.Open();
            using var comando = new MySqlCommand("DELETE FROM clientes WHERE id=@id", conexion);
            comando.Parameters.AddWithValue("@id", id); comando.ExecuteNonQuery();
        }
        private static void Ejecutar(string sql, Cliente cliente)
        {
            using var conexion = Conexion.Obtener(); conexion.Open();
            using var comando = new MySqlCommand(sql, conexion);
            comando.Parameters.AddWithValue("@id", cliente.Id); comando.Parameters.AddWithValue("@nombre", cliente.Nombre);
            comando.Parameters.AddWithValue("@telefono", cliente.Telefono); comando.Parameters.AddWithValue("@correo", cliente.Correo);
            comando.ExecuteNonQuery();
        }
    }
}
