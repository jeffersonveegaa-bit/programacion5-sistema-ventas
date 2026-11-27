using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using Datos;
using Entidades;
using MySql.Data.MySqlClient;

namespace Negocio
{
    /// <summary> Reglas de negocio de clientes. </summary>
    public class ClienteService
    {
        private readonly ClienteDAO _dao = RepositorioFactory.Clientes();
        public List<Cliente> Listar() => _dao.ObtenerTodos();
        public static void Validar(Cliente c)
        {
            if (string.IsNullOrWhiteSpace(c.Nombre)) throw new ArgumentException("El nombre es obligatorio.");
            if (!Regex.IsMatch(c.Correo ?? "", @"^\S+@\S+\.\S+$")) throw new ArgumentException("Correo electrónico inválido.");
        }
        public void Guardar(Cliente c) { Validar(c); if (c.Id == 0) _dao.Insertar(c); else _dao.Actualizar(c); }
        //public void Eliminar(int id) => _dao.Eliminar(id);
        public void Eliminar(int id)
        {
            try
            {
                _dao.Eliminar(id);
            }
            catch (MySqlException ex)
            {
                if (ex.Number == 1451)
                {
                    throw new InvalidOperationException(
                        "No se puede eliminar el cliente porque tiene ventas asociadas.");
                }

                throw;
            }
        }
    }
}
