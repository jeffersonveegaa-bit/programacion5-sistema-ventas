using System.Collections.Generic;
using Entidades;
using MySql.Data.MySqlClient;

namespace Datos
{
    /// <summary> DAO de productos (consultas parametrizadas contra SQL Injection) </summary>
    public class ProductoDAO : IRepositorio<Producto, string>
    {
        public List<Producto> ObtenerTodos()
        {
            var lista = new List<Producto>();
            using var conexion = Conexion.Obtener(); conexion.Open();
            using var comando = new MySqlCommand("SELECT codigo,nombre,descripcion,precio,stock FROM productos", conexion);
            using var lector = comando.ExecuteReader();
            while (lector.Read())
                lista.Add(new Producto { Codigo = lector.GetString(0), Nombre = lector.GetString(1), Descripcion = lector.IsDBNull(2) ? "" : lector.GetString(2), Precio = lector.GetDecimal(3), Stock = lector.GetInt32(4) });
            return lista;
        }

        public void Insertar(Producto producto) => Ejecutar("INSERT INTO productos VALUES (@codigo,@nombre,@descripcion,@precio,@stock)", producto);
        public void Actualizar(Producto producto) => Ejecutar("UPDATE productos SET nombre=@nombre,descripcion=@descripcion,precio=@precio,stock=@stock WHERE codigo=@codigo", producto);
        public void Eliminar(string codigo)
        {
            using var conexion = Conexion.Obtener(); conexion.Open();
            using var comando = new MySqlCommand("DELETE FROM productos WHERE codigo=@codigo", conexion);
            comando.Parameters.AddWithValue("@codigo", codigo); comando.ExecuteNonQuery();
        }
        private static void Ejecutar(string sql, Producto p)
        {
            using var conexion = Conexion.Obtener(); conexion.Open();
            using var comando = new MySqlCommand(sql, conexion);
            comando.Parameters.AddWithValue("@codigo", p.Codigo); comando.Parameters.AddWithValue("@nombre", p.Nombre);
            comando.Parameters.AddWithValue("@descripcion", p.Descripcion); comando.Parameters.AddWithValue("@precio", p.Precio);
            comando.Parameters.AddWithValue("@stock", p.Stock); comando.ExecuteNonQuery();
        }
    }
}
